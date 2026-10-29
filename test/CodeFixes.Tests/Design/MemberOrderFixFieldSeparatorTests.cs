using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

/// <summary>
/// Regression tests for Airframe#372: <see cref="MemberOrderFix.Separate"/> unconditionally
/// inserts a blank-line separator into the leading trivia of every member but the first,
/// regardless of member kind. Two consecutive single-line field declarations are exempt from
/// SA1516's "blank line between members" rule, so that unconditional insert manufactures a blank
/// line the input never had and the enforced ruleset never asked for.
///
/// These tests pin the corrected rule directly against the enforced SA15xx family
/// (analyzers.ruleset lines 236-249, all at Error):
///   - SA1516: no blank line required between two consecutive single-line fields (or event
///     fields); required at every other adjacency.
///   - SA1514 / SA1515: a member preceded by a doc comment or a `//` comment still requires its
///     blank line, even when the pair is otherwise field-field.
///   - SA1505: the first member never gets a leading blank line after the opening brace.
///   - SA1507: never two blank lines in a row, anywhere in the fixed output.
/// </summary>
public class MemberOrderFixFieldSeparatorTests
{
    /// <summary>
    /// The bug. Three consecutive <c>private readonly</c> fields with no blank lines between them
    /// in the input, reordered behind a constructor that starts life above them. The current
    /// implementation inserts a blank line into the leading trivia of every member but the first,
    /// so the three fields end up separated even though SA1516 exempts consecutive single-line
    /// fields and the input never had blank lines there to begin with.
    /// </summary>
    [Theory]
    [InlineData(ThreeConsecutiveFieldsBeforeConstructor)]
    public async Task GivenThreeConsecutiveFieldsBeforeConstructor_WhenCodeFixApplied_ThenFieldsRemainPackedWithNoBlankLines(string source)
    {
        // Given, When
        var fixedText = await ResolveFixedSourceAsync(source);

        // Then. SA1516 exempts consecutive single-line fields from the blank-line requirement; the
        // fix must not manufacture separators the input never had between them.
        var expectedPackedFields = Normalize(
            """
                    private readonly ICqrs _cqrs;
                    private readonly IHyperMediaBuilder _hyperMediaBuilder;
                    private readonly ILogger _logger;
            """);

        fixedText.Should().Contain(
            expectedPackedFields,
            "SA1516 exempts consecutive single-line fields from the blank-line rule, and the input never had blank lines between these three fields");
    }

    /// <summary>
    /// Guard: a non-field adjacency (property followed by method) must keep its separating blank
    /// line. Pins SA1516 from the other direction, so a fix for the bug above cannot become
    /// over-broad and strip every separator regardless of member kind.
    /// </summary>
    [Theory]
    [InlineData(PropertyAdjacentToMethod)]
    public async Task GivenPropertyAdjacentToMethod_WhenCodeFixApplied_ThenBlankLineSeparatesThem(string source)
    {
        // Given, When
        var fixedText = await ResolveFixedSourceAsync(source);

        // Then
        var expectedSeparatedPair = Normalize(
            """
                    public int Property { get; set; }

                    public void Method()
            """);

        fixedText.Should().Contain(
            expectedSeparatedPair,
            "a property and a method are not both field-like, so SA1516 still requires a blank line between them");
    }

    /// <summary>
    /// Guard: two adjacent <c>public</c> fields must still be separated by a blank line. SA1516
    /// itself would exempt this pair too, but the exemption is deliberately scoped to private
    /// fields only — packing implementation-detail fields together reads fine, packing public API
    /// surface does not.
    /// </summary>
    [Theory]
    [InlineData(PublicFieldsBeforeConstructor)]
    public async Task GivenAdjacentPublicFields_WhenCodeFixApplied_ThenBlankLineSeparatesThem(string source)
    {
        // Given, When
        var fixedText = await ResolveFixedSourceAsync(source);

        // Then
        var expectedSeparatedFields = Normalize(
            """
                    public int First;

                    public int Second;
            """);

        fixedText.Should().Contain(
            expectedSeparatedFields,
            "the field-field exemption is scoped to private fields, so adjacent public fields still get a separating blank line");
    }

    /// <summary>
    /// Guard: two adjacent private fields where the second carries an XML doc comment must still
    /// be separated by a blank line. Pins SA1514, which is not conditioned on member kind the way
    /// SA1516's field-field exemption is.
    /// </summary>
    [Theory]
    [InlineData(FieldFollowedByDocumentedField)]
    public async Task GivenFieldFollowedByDocumentedField_WhenCodeFixApplied_ThenBlankLinePrecedesDocComment(string source)
    {
        // Given, When
        var fixedText = await ResolveFixedSourceAsync(source);

        // Then
        var expectedDocumentedFieldBlock = Normalize(
            """
                    private readonly int _first;

                    /// <summary>Second field.</summary>
                    private readonly int _second;
            """);

        fixedText.Should().Contain(
            expectedDocumentedFieldBlock,
            "SA1514 requires a blank line before a documented member regardless of whether the preceding member is also a field");
    }

    /// <summary>
    /// Guard: the first member after reordering never gets a leading blank line. Pins SA1505 —
    /// the line that used to separate the original first member from the opening brace must not
    /// travel with whichever member the sort now places first.
    /// </summary>
    [Theory]
    [InlineData(ThreeConsecutiveFieldsBeforeConstructor)]
    public async Task GivenReorderedType_WhenCodeFixApplied_ThenFirstMemberHasNoLeadingBlankLine(string source)
    {
        // Given, When
        var fixedText = await ResolveFixedSourceAsync(source);

        // Then
        var expectedBraceThenFirstMember = Normalize(
            """
                {
                    public SearchController()
            """);

        fixedText.Should().Contain(
            expectedBraceThenFirstMember,
            "SA1505 forbids a blank line immediately after the opening brace, so the reordered first member must sit directly beneath it");
    }

    /// <summary>
    /// Guard: the fixed output never contains two blank lines in a row, anywhere. Pins SA1507
    /// independent of which members ended up adjacent to which.
    /// </summary>
    [Theory]
    [InlineData(ThreeConsecutiveFieldsBeforeConstructor)]
    [InlineData(FieldFollowedByDocumentedField)]
    [InlineData(PropertyAdjacentToMethod)]
    public async Task GivenAnyReorderedType_WhenCodeFixApplied_ThenNoDoubleBlankLineAppears(string source)
    {
        // Given, When
        var fixedText = await ResolveFixedSourceAsync(source);

        // Then. Three consecutive newline characters is two blank lines in a row.
        fixedText.Should().NotContain("\n\n\n", "SA1507 forbids two consecutive blank lines anywhere in the fixed output");
    }

    private static async Task<string> ResolveFixedSourceAsync(string source)
    {
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithCodeFix<MemberOrderFix>()
           .AddNormalizedSources(source)
           .GenerateAsync();

        var resolvedFix = result.CodeFixResults[typeof(MemberOrderFix)].ResolvedFixes.Single();
        var codeAction = resolvedFix.CodeActions.Single();
        var textChange = codeAction.TextChanges.Single().Value;
        var originalText = await resolvedFix.Document.GetTextAsync();

        return originalText.WithChanges(textChange).ToString();
    }

    /// <summary>
    /// Normalizes an expectation literal to the same line ending the sources are normalized to
    /// (see <see cref="GeneratorTestContextBuilderExtensions.AddNormalizedSources"/>), so the
    /// comparison does not depend on whether this file's raw string literals were checked out or
    /// compiled with CRLF.
    /// </summary>
    private static string Normalize(string value) => value.ReplaceLineEndings("\n");

    /// <summary>
    /// The bug report's exact reproduction: three consecutive <c>private readonly</c> fields, no
    /// blank lines between them, followed by a parameterless constructor. RSA2001 flags the
    /// constructor for sitting below the fields; the fix moves it above them.
    /// </summary>
    // lang=csharp
    internal const string ThreeConsecutiveFieldsBeforeConstructor =
        """
        namespace Sample
        {
            public class SearchController
            {
                private readonly ICqrs _cqrs;
                private readonly IHyperMediaBuilder _hyperMediaBuilder;
                private readonly ILogger _logger;

                public SearchController()
                {
                }
            }
        }
        """;

    /// <summary>
    /// A property and a method, in kind order already, sitting ahead of a misplaced constructor
    /// so RSA2001 still fires and the fix still runs <see cref="MemberOrderFix.Separate"/> over
    /// the whole member list.
    /// </summary>
    // lang=csharp
    internal const string PropertyAdjacentToMethod =
        """
        namespace Sample
        {
            public class Example
            {
                public int Property { get; set; }
                public void Method()
                {
                }

                public Example()
                {
                }
            }
        }
        """;

    /// <summary>
    /// Two <c>public</c> fields with no blank line between them in the input, sitting ahead of a
    /// misplaced constructor.
    /// </summary>
    // lang=csharp
    internal const string PublicFieldsBeforeConstructor =
        """
        namespace Sample
        {
            public class Example
            {
                public int First;
                public int Second;

                public Example()
                {
                }
            }
        }
        """;

    /// <summary>
    /// Two private fields with no blank line between them in the input, the second carrying an XML
    /// doc comment, sitting ahead of a misplaced constructor.
    /// </summary>
    // lang=csharp
    internal const string FieldFollowedByDocumentedField =
        """
        namespace Sample
        {
            public class Example
            {
                private readonly int _first;
                /// <summary>Second field.</summary>
                private readonly int _second;

                public Example()
                {
                }
            }
        }
        """;
}
