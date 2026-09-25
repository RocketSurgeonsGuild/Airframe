using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

public class MemberOrderFixTests
{
    [Theory]
    [InlineData(nameof(ConstructorAfterField), ConstructorAfterField)]
    [InlineData(nameof(Unordered), Unordered)]
    [InlineData(nameof(DocumentedMembers), DocumentedMembers)]
    [InlineData(nameof(UnsortedUsings), UnsortedUsings)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithCodeFix<MemberOrderFix>()
           .AddNormalizedSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    [Theory]
    [InlineData(ConstructorAfterField)]
    public async Task GivenConstructorAfterField_WhenCodeFix_ThenFixResolved(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithCodeFix<MemberOrderFix>()
           .AddNormalizedSources(source)
           .Build()
           .GenerateAsync();

        // Then
        result
           .CodeFixResults[typeof(MemberOrderFix)]
           .ResolvedFixes
           .Should()
           .ContainSingle()
           .Which
           .Diagnostic
           .Descriptor
           .Should()
           .Be(Descriptions.RSA2001);
    }

    [Theory]
    [InlineData(UnsortedUsings)]
    public async Task GivenUnsortedUsings_WhenCodeFix_ThenUsingsSorted(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithCodeFix<MemberOrderFix>()
           .AddNormalizedSources(source)
           .Build()
           .GenerateAsync();

        var resolvedFix = result.CodeFixResults[typeof(MemberOrderFix)].ResolvedFixes.Single();
        var codeAction = resolvedFix.CodeActions[0];
        var textChange = codeAction.TextChanges.Single().Value;
        var originalText = await resolvedFix.Document.GetTextAsync();
        var fixedText = originalText.WithChanges(textChange).ToString();

        // Then. System namespaces first (alphabetically), then the static directive, then the alias.
        var systemIndex = fixedText.IndexOf("using System;", StringComparison.Ordinal);
        var collectionsIndex = fixedText.IndexOf("using System.Collections.Generic;", StringComparison.Ordinal);
        var textIndex = fixedText.IndexOf("using System.Text;", StringComparison.Ordinal);
        var staticIndex = fixedText.IndexOf("using static System.Math;", StringComparison.Ordinal);
        var aliasIndex = fixedText.IndexOf("using X = System.Exception;", StringComparison.Ordinal);

        systemIndex.Should().BeGreaterThanOrEqualTo(0).And.BeLessThan(collectionsIndex);
        collectionsIndex.Should().BeLessThan(textIndex);
        textIndex.Should().BeLessThan(staticIndex);
        staticIndex.Should().BeLessThan(aliasIndex);
    }

    [Theory]
    [InlineData(DirectiveSpanningMembers)]
    [InlineData(DirectiveClosingOnTypeBrace)]
    [InlineData(PragmaSpanningMembers)]
    public async Task GivenDirectiveAcrossMembers_WhenCodeFix_ThenNoFixOffered(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithCodeFix<MemberOrderFix>()
           .AddNormalizedSources(source)
           .Build()
           .GenerateAsync();

        // Then. The diagnostic still stands; only the automatic edit declines, because reordering
        // would move a member out of the conditional block and change which symbols compile it.
        result
           .AnalyzerResults[typeof(Rsa2001)]
           .Diagnostics
           .Should()
           .NotBeEmpty();

        result
           .CodeFixResults[typeof(MemberOrderFix)]
           .ResolvedFixes
           .SelectMany(fix => fix.CodeActions)
           .Should()
           .BeEmpty();
    }

    [Theory]
    [InlineData(DirectiveWithinOneMember)]
    public async Task GivenDirectiveWithinOneMember_WhenCodeFix_ThenFixOffered(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithCodeFix<MemberOrderFix>()
           .AddNormalizedSources(source)
           .Build()
           .GenerateAsync();

        // Then. The block travels with the member that carries it, so the reorder is safe.
        result
           .CodeFixResults[typeof(MemberOrderFix)]
           .ResolvedFixes
           .SelectMany(fix => fix.CodeActions)
           .Should()
           .NotBeEmpty();
    }

    // lang=csharp
    internal const string ConstructorAfterField =
        """
        namespace Sample
        {
            public class Example
            {
                private readonly int _value;

                public Example()
                {
                }
            }
        }
        """;

    // lang=csharp
    internal const string Unordered =
        """
        namespace Sample
        {
            public class Example
            {
                private void Helper()
                {
                }

                public void Method()
                {
                }

                public int Property { get; set; }

                private int _value;

                public Example()
                {
                }
            }
        }
        """;

    /// <summary>
    /// Usings out of order alongside a member ordering violation, so one fix pass resolves both:
    /// <c>System</c> namespaces first, then other regular usings, both alphabetically; a
    /// <c>using static</c> directive after them; the alias last.
    /// </summary>
    // lang=csharp
    internal const string UnsortedUsings =
        """
        using System.Text;
        using X = System.Exception;
        using static System.Math;
        using System.Collections.Generic;
        using System;

        namespace Sample
        {
            public class Example
            {
                private readonly int _value;

                public Example()
                {
                }
            }
        }
        """;

    // lang=csharp
    internal const string DocumentedMembers =
        """
        namespace Sample
        {
            /// <summary>An example.</summary>
            public class Example
            {
                /// <summary>Helps.</summary>
                private void Helper()
                {
                }

                /// <summary>Creates the example.</summary>
                public Example()
                {
                }
            }
        }
        """;

    /// <summary>
    /// Both members sit inside the conditional block, so reordering would lift the constructor
    /// out of it.
    /// </summary>
    // lang=csharp
    internal const string DirectiveSpanningMembers =
        """
        namespace Sample
        {
            public class Example
            {
        #if !NEVER
                private int _value;

                public Example()
                {
                }
        #endif
            }
        }
        """;

    /// <summary>
    /// The block opens inside a member and closes on the type's own brace.
    /// </summary>
    // lang=csharp
    internal const string DirectiveClosingOnTypeBrace =
        """
        namespace Sample
        {
            public class Example
            {
                private int _value;

        #if !NEVER
                public Example()
                {
                }
        #endif
            }
        }
        """;

    /// <summary>
    /// The whole block is leading trivia of one member, so it moves with that member.
    /// </summary>
    // lang=csharp
    internal const string DirectiveWithinOneMember =
        """
        namespace Sample
        {
            public class Example
            {
                private int _value;

        #if NEVER
                private int _disabled;
        #endif
                public Example()
                {
                }
            }
        }
        """;

    /// <summary>
    /// A pragma pair wrapping fields. RSA2012 does not report this, but moving a member across it
    /// would still change which warnings are suppressed, so the reorder declines all the same.
    /// </summary>
    // lang=csharp
    internal const string PragmaSpanningMembers =
        """
        namespace Sample
        {
            public class Example
            {
        #pragma warning disable CA2213
                private int _first;
        #pragma warning restore CA2213

                public Example()
                {
                }
            }
        }
        """;
}
