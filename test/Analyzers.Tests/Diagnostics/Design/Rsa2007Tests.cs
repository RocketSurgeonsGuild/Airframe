using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Design;

public class Rsa2007Tests
{
    [Theory]
    [InlineData(DesignTestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2007>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA2007.Id));
    }

    [Theory]
    [InlineData(DesignTestData.MutableBeforeReadonly)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2007>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa2007)]
           .Diagnostics
           .Should()
           .ContainSingle()
           .Which
           .Id
           .Should()
           .Be(RSA2007.Id);
    }

    [Fact]
    public async Task GivenAnotherRulesViolation_WhenAnalyze_ThenNoDiagnosticsReported()
    {
        // Given
        var sources = DesignTestData.Ordering.Where(source => source != DesignTestData.MutableBeforeReadonly);

        foreach (var source in sources)
        {
            // When
            var result = await GeneratorTestContextBuilder
               .Create()
               .AddSources(source)
               .WithAnalyzer<Rsa2007>()
               .GenerateAsync();

            // Then
            result
               .AnalyzerResults[typeof(Rsa2007)]
               .Diagnostics
               .Should()
               .NotContain(diagnostic => diagnostic.Id == RSA2007.Id, "only one rule owns a rank component");
        }
    }

    [Theory]
    [InlineData(nameof(DesignTestData.Correct), DesignTestData.Correct)]
    [InlineData(nameof(DesignTestData.MutableBeforeReadonly), DesignTestData.MutableBeforeReadonly)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2007>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }
}
