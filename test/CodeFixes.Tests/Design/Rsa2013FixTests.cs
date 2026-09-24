using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

public class Rsa2013FixTests
{
    [Theory]
    [InlineData(nameof(LongParameterList), LongParameterList)]
    [InlineData(nameof(LongArgumentList), LongArgumentList)]
    [InlineData(nameof(LongCollectionExpression), LongCollectionExpression)]
    [InlineData(nameof(LongExpressionBody), LongExpressionBody)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2013>()
           .WithCodeFix<Rsa2013Fix>()
           .AddNormalizedSources(source)
           .AddGlobalOption("max_line_length", "80")
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    [Theory]
    [InlineData(LongStringLiteral)]
    public async Task GivenLengthIsAString_WhenCodeFix_ThenNoFixOffered(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2013>()
           .WithCodeFix<Rsa2013Fix>()
           .AddNormalizedSources(source)
           .AddGlobalOption("max_line_length", "80")
           .Build()
           .GenerateAsync();

        // Then. The line is long because of its text, and there is no break to insert.
        result
           .AnalyzerResults[typeof(Rsa2013)]
           .Diagnostics
           .Should()
           .NotBeEmpty();

        result
           .CodeFixResults[typeof(Rsa2013Fix)]
           .ResolvedFixes
           .SelectMany(fix => fix.CodeActions)
           .Should()
           .BeEmpty();
    }

    // lang=csharp
    internal const string LongParameterList =
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
    internal const string LongArgumentList =
        """
        namespace Sample
        {
            public class Example
            {
                public string Value { get; } = string.Join("-", "alpha", "bravo", "charlie", "delta");
            }
        }
        """;

    // lang=csharp
    internal const string LongCollectionExpression =
        """
        namespace Sample
        {
            public class Example
            {
                private static readonly string[] Names = ["alpha", "bravo", "charlie", "delta", "echo"];
            }
        }
        """;

    // lang=csharp
    internal const string LongExpressionBody =
        """
        namespace Sample
        {
            public class Example
            {
                public string Combine(string first) => first + "a rather long suffix indeed here";
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
                private const string Message = "a single very long string literal that cannot be broken apart";
            }
        }
        """;
}
