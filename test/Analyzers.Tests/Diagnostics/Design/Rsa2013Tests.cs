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
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2013>()
           .GenerateAsync();

        // Then
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

    // Bug #371: comment and XML-doc trivia is excluded from the measured length.
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

        // Then
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

        // Then
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

        // Then
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

        // Then
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

        // Then
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

        // Then
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

        // Then
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

        // Then
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .BeEmpty();
    }

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

        // Then
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

        // Then
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

        // Then
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

    /// <summary>
    /// The overlong line is an interior line of a delimited <c>/* */</c> comment, not the opener.
    /// </summary>
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

    /// <summary>
    /// Concatenated rather than a raw string literal: the trailing whitespace after the comment's
    /// closing <c>*/</c> is the point of this fixture, and a raw literal's dedent is not guaranteed
    /// to preserve it.
    /// </summary>
    internal const string LongBlockCommentAloneWithTrailingWhitespace =
        "namespace Sample\n" +
        "{\n" +
        "    public class Example\n" +
        "    {\n" +
        "        /* This block comment sits alone on its own line, long enough to sail past the eighty character margin. */   \n" +
        "        public string Value { get; set; }\n" +
        "    }\n" +
        "}";

    /// <summary>
    /// The FIRST comment in the run is the long one, deliberately, so the prefix up to the start of
    /// the LAST trivia is already past the margin on its own. A fixture where only the trailing
    /// trivia were long would pass against the old single-trivia algorithm and guard nothing.
    /// </summary>
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

    /// <summary>
    /// Concatenated rather than a raw string literal, for the same reason as
    /// <see cref="LongBlockCommentAloneWithTrailingWhitespace"/>: the trailing whitespace after the
    /// comment must survive exactly as written.
    /// </summary>
    internal const string LongTrailingBlockCommentWithTrailingWhitespace =
        "namespace Sample\n" +
        "{\n" +
        "    public class Example\n" +
        "    {\n" +
        "        public string Value { get; set; } /* trailing block comment padded so the raw line sails past the eighty character margin */   \n" +
        "    }\n" +
        "}";

    /// <summary>
    /// Same reasoning as <see cref="LongTwoCommentRunAloneOnOwnLine"/>: the FIRST trivia in the run
    /// is the long one, so the prefix up to the trailing <c>// y</c> is already past the margin.
    /// </summary>
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

    /// <summary>
    /// The declaration's code content alone is 94 characters, past the margin before the trailing
    /// comment is even considered, and the raw line is 111. The reported length must be 94: a
    /// developer must not be able to silence a genuinely overlong code line by appending a comment.
    /// </summary>
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

    /// <summary>
    /// The <c>/* flag */</c> comment sits before code that keeps going after it, so it is not a
    /// trailing run and is not excluded. The reported length must be the full 91.
    /// </summary>
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

    /// <summary>
    /// String literals are deliberately out of scope for the comment-trivia carve-out.
    /// </summary>
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
