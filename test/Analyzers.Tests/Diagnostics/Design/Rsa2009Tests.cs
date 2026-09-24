using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Design;

public class Rsa2009Tests
{
    [Theory]
    [InlineData(DesignTestData.FileNameMatchesType)]
    [InlineData(DesignTestData.PartialContinuation)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2009>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA2009.Id));
    }

    [Theory]
    [InlineData(DesignTestData.FileNameDoesNotMatchType)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2009>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa2009)]
           .Diagnostics
           .Should()
           .ContainSingle()
           .Which
           .Id
           .Should()
           .Be(RSA2009.Id);
    }

    [Theory]
    [InlineData(nameof(DesignTestData.FileNameMatchesType), DesignTestData.FileNameMatchesType)]
    [InlineData(nameof(DesignTestData.FileNameDoesNotMatchType), DesignTestData.FileNameDoesNotMatchType)]
    [InlineData(nameof(DesignTestData.PartialContinuation), DesignTestData.PartialContinuation)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2009>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }
}
