using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Design;

public class Rsa2013Tests
{
    [Theory]
    [InlineData(ShortLines)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA2013.Id));
    }

    [Theory]
    [InlineData(LongLine)]
    public async Task GivenLongLine_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .ContainSingle()
           .Which
           .Id
           .Should()
           .Be(RSA2013.Id);
    }

    [Theory]
    [InlineData(LongLine)]
    public async Task GivenNoConfiguredLimit_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When. No max_line_length is supplied.
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. The rule takes its limit from configuration rather than inventing one, so with
        // nothing configured it has nothing to say.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    [Theory]
    [InlineData(LongLine)]
    public async Task GivenLimitIsOff_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "off")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    // ---------------------------------------------------------------------------------------
    // Bug #371: DocumentWalk.Compute used to measure every raw SourceText line with zero trivia
    // awareness, so comment and XML-doc lines got measured as plain text and reported by
    // RSA2013, and Rsa2013Fix declines all of them (nothing to chop), so the warning had no
    // remedy but suppression. These four report NOTHING now that the walk excludes a line's
    // trailing comment/doc-comment trivia from the measured length.
    // ---------------------------------------------------------------------------------------

    [Theory]
    [InlineData(LongXmlDocSummaryLine)]
    public async Task GivenLongXmlDocSummaryLine_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. A `///` prose line that overruns the margin is documentation, not code, so it
        // must not be reported.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    [Theory]
    [InlineData(LongSingleLineComment)]
    public async Task GivenLongSingleLineComment_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. A `//` line that is entirely comment has no code content, so it must not be
        // reported.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    [Theory]
    [InlineData(LongLineInsideBlockComment)]
    public async Task GivenLongLineInsideBlockComment_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. The overlong line is an interior line of a delimited /* ... */ comment (not the
        // opener), so it is still entirely comment and must not be reported.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    [Theory]
    [InlineData(LongBlockCommentAloneWithTrailingWhitespace)]
    public async Task GivenLongBlockCommentAloneWithTrailingWhitespace_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. Trailing whitespace after the comment's closing `*/` must not stop the walk
        // from recognizing the comment as reaching the end of the line — the line is still
        // entirely comment (plus trailing whitespace) and must not be reported.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    [Theory]
    [InlineData(LongTwoCommentRunAloneOnOwnLine)]
    public async Task GivenLongTwoCommentRunAloneOnOwnLine_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. A trailing comment RUN can be more than one trivia (`/* x */ // y`) — the walk
        // must peel back through both, not just the last one, so a line that is entirely a
        // comment run must not be reported.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    [Theory]
    [InlineData(LongTrailingCommentOnShortCode)]
    public async Task GivenLongTrailingCommentOnShortCode_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. The declaration's code content ("public string Value { get; set; }", 41 chars)
        // is comfortably within the margin; only the trailing // comment pushes the raw line
        // past 80. Excluding trailing comment trivia from the measured length means this must
        // not be reported.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    [Theory]
    [InlineData(LongTrailingBlockCommentWithTrailingWhitespace)]
    public async Task GivenLongTrailingBlockCommentWithTrailingWhitespace_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. Trailing whitespace after a trailing block comment's `*/` must not stop the
        // walk from finding it: the declaration's code content is comfortably within the
        // margin, and must not be reported.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    [Theory]
    [InlineData(LongTrailingTwoCommentRun)]
    public async Task GivenLongTrailingTwoCommentRun_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. A trailing comment run of more than one trivia (`/* x */ // y`) after short code
        // must be peeled back through entirely, not just its last trivia, so this must not be
        // reported.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    // ---------------------------------------------------------------------------------------
    // Regression guards: these pin down that the fix does not over-reach — comment trivia is
    // excluded from the measured length, but that is the ONLY thing excluded.
    // ---------------------------------------------------------------------------------------

    [Theory]
    [InlineData(LongCodeWithTrailingComment)]
    public async Task GivenLongCodeWithTrailingComment_WhenAnalyze_ThenDiagnosticReflectsCodeContentLength(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. The declaration's code content alone ("public static string Combine(string
        // first, string second, string third, string fourth)", 94 chars) already exceeds the
        // margin, before the trailing "// trailing note" comment is even considered. A
        // developer must not be able to silence a genuinely overlong code line by appending a
        // comment, so this must still report — and the reported length must be the CODE
        // CONTENT length (94), not the raw line length including the comment (111).
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .ContainSingle()
           .Which
           .GetMessage()
           .Should()
           .Be("Line is 94 characters long; the maximum is 80");
    }

    [Theory]
    [InlineData(LongMethodWithMidLineComment)]
    public async Task GivenLongMethodWithMidLineComment_WhenAnalyze_ThenDiagnosticReflectsFullLineLength(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. The `/* flag */` comment sits BEFORE code that keeps going after it on the same
        // line, so it is not a trailing run — only a comment run reaching the end of the line is
        // excluded. This must still report, and the reported length must be the full raw line
        // length (91), not a length that has had the interior comment subtracted out.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .ContainSingle()
           .Which
           .GetMessage()
           .Should()
           .Be("Line is 91 characters long; the maximum is 80");
    }

    [Theory]
    [InlineData(LongStringLiteral)]
    public async Task GivenLongStringLiteral_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. String literals are deliberately out of scope for the comment-trivia carve-out
        // — a long string-literal line must still be reported, in full.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .ContainSingle()
           .Which
           .Id
           .Should()
           .Be(RSA2013.Id);
    }

    [Theory]
    [InlineData(ShortDocCommentAndShortCode)]
    public async Task GivenShortDocCommentAndShortCode_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then. Sanity guard: a doc-comment line within the margin, on a class whose code lines
        // are all within the margin, reports nothing.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

    [Theory]
    [InlineData(nameof(ShortLines), ShortLines)]
    [InlineData(nameof(LongLine), LongLine)]
    [InlineData(nameof(LongXmlDocSummaryLine), LongXmlDocSummaryLine)]
    [InlineData(nameof(LongSingleLineComment), LongSingleLineComment)]
    [InlineData(nameof(LongLineInsideBlockComment), LongLineInsideBlockComment)]
    [InlineData(nameof(LongBlockCommentAloneWithTrailingWhitespace), LongBlockCommentAloneWithTrailingWhitespace)]
    [InlineData(nameof(LongTwoCommentRunAloneOnOwnLine), LongTwoCommentRunAloneOnOwnLine)]
    [InlineData(nameof(LongTrailingCommentOnShortCode), LongTrailingCommentOnShortCode)]
    [InlineData(nameof(LongTrailingBlockCommentWithTrailingWhitespace), LongTrailingBlockCommentWithTrailingWhitespace)]
    [InlineData(nameof(LongTrailingTwoCommentRun), LongTrailingTwoCommentRun)]
    [InlineData(nameof(LongCodeWithTrailingComment), LongCodeWithTrailingComment)]
    [InlineData(nameof(LongMethodWithMidLineComment), LongMethodWithMidLineComment)]
    [InlineData(nameof(LongStringLiteral), LongStringLiteral)]
    [InlineData(nameof(ShortDocCommentAndShortCode), ShortDocCommentAndShortCode)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2013>()
           .AddSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    // lang=csharp
    internal const string ShortLines =
        """
        namespace Sample
        {
            public class Example
            {
                public string Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string LongLine =
        """
        namespace Sample
        {
            public class Example
            {
                public static string Combine(string first, string second, string third, string fourth)
                {
                    return first;
                }
            }
        }
        """;

    // lang=csharp
    internal const string LongXmlDocSummaryLine =
        """
        namespace Sample
        {
            public class Example
            {
                /// <summary>
                /// This summary sentence is intentionally long enough to sail well past the eighty character margin on its own.
                /// </summary>
                public string Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string LongSingleLineComment =
        """
        namespace Sample
        {
            public class Example
            {
                // This single line comment is intentionally long enough to sail past the eighty character margin here.
                public string Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string LongLineInsideBlockComment =
        """
        namespace Sample
        {
            public class Example
            {
                /*
                 * This interior block comment line is intentionally long enough to sail past the eighty character margin.
                 */
                public string Value { get; set; }
            }
        }
        """;

    // Uses string concatenation rather than a raw string literal: trailing whitespace after the
    // comment on the fourth line is the whole point of this fixture, and a raw string literal's
    // dedent/trim behavior is not guaranteed to preserve it.
    internal const string LongBlockCommentAloneWithTrailingWhitespace =
        "namespace Sample\n" +
        "{\n" +
        "    public class Example\n" +
        "    {\n" +
        "        /* This block comment sits alone on its own line, long enough to sail past the eighty character margin. */   \n" +
        "        public string Value { get; set; }\n" +
        "    }\n" +
        "}";

    // The FIRST comment in the run is the long one, deliberately, so that the prefix up to the
    // start of the LAST trivia is already past the margin on its own. A run fixture where only
    // the trailing trivia is long cannot distinguish this from the old single-trivia algorithm
    // (which excluded exactly that last trivia and would have measured the same short prefix) —
    // see GivenLongTwoCommentRunAloneOnOwnLine_WhenAnalyze_ThenNoDiagnosticsReported.
    // lang=csharp
    internal const string LongTwoCommentRunAloneOnOwnLine =
        """
        namespace Sample
        {
            public class Example
            {
                /* This first block comment is now the long one, made long enough to push the prefix well past the eighty character margin all on its own */ // trailing note
                public string Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string LongTrailingCommentOnShortCode =
        """
        namespace Sample
        {
            public class Example
            {
                public string Value { get; set; } // a trailing comment padded out so the raw line sails past the eighty character margin
            }
        }
        """;

    // Uses string concatenation rather than a raw string literal, for the same reason as
    // LongBlockCommentAloneWithTrailingWhitespace above: the trailing whitespace after the
    // comment must survive exactly as written.
    internal const string LongTrailingBlockCommentWithTrailingWhitespace =
        "namespace Sample\n" +
        "{\n" +
        "    public class Example\n" +
        "    {\n" +
        "        public string Value { get; set; } /* trailing block comment padded so the raw line sails past the eighty character margin */   \n" +
        "    }\n" +
        "}";

    // Same reasoning as LongTwoCommentRunAloneOnOwnLine above: the FIRST trivia in the run (the
    // block comment) is the long one, so the prefix up to the start of the trailing "// y" is
    // already past the margin — a fixture where only the trailing trivia were long would pass
    // against the old single-trivia algorithm too and guard nothing.
    // lang=csharp
    internal const string LongTrailingTwoCommentRun =
        """
        namespace Sample
        {
            public class Example
            {
                public string Value { get; set; } /* This block comment is now the long one, long enough to push the prefix past the eighty character margin */ // y
            }
        }
        """;

    // lang=csharp
    internal const string LongCodeWithTrailingComment =
        """
        namespace Sample
        {
            public class Example
            {
                public static string Combine(string first, string second, string third, string fourth) // trailing note
                {
                    return first;
                }
            }
        }
        """;

    // lang=csharp
    internal const string LongMethodWithMidLineComment =
        """
        namespace Sample
        {
            public class Example
            {
                public void LongMethodName(/* flag */ int first, int second, int third, int fourth)
                {
                }
            }
        }
        """;

    // lang=csharp
    internal const string LongStringLiteral =
        """
        namespace Sample
        {
            public class Example
            {
                public const string Value = "this string literal is intentionally long enough to sail past the eighty character margin";
            }
        }
        """;

    // lang=csharp
    internal const string ShortDocCommentAndShortCode =
        """
        namespace Sample
        {
            /// <summary>Short summary.</summary>
            public class Example
            {
                public string Value { get; set; }
            }
        }
        """;
}
