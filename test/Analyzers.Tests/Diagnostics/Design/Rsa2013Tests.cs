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

    [Theory]
    [InlineData(nameof(ShortLines), ShortLines)]
    [InlineData(nameof(LongLine), LongLine)]
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
}
