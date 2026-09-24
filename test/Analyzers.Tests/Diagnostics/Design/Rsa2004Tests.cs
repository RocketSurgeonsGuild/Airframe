using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Design;

public class Rsa2004Tests
{
    [Theory]
    [InlineData(DesignTestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2004>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA2004.Id));
    }

    [Theory]
    [InlineData(DesignTestData.InternalBeforePublic)]
    [InlineData(DesignTestData.ProtectedBeforePublic)]
    [InlineData(DesignTestData.ProtectedInternalBeforePublic)]
    [InlineData(DesignTestData.PrivateProtectedBeforePublic)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2004>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa2004)]
           .Diagnostics
           .Should()
           .ContainSingle()
           .Which
           .Id
           .Should()
           .Be(RSA2004.Id);
    }

    [Fact]
    public async Task GivenAnotherRulesViolation_WhenAnalyze_ThenNoDiagnosticsReported()
    {
        // Given
        var sources = DesignTestData.Ordering.Where(source => source != DesignTestData.InternalBeforePublic);

        foreach (var source in sources)
        {
            // When
            var result = await GeneratorTestContextBuilder
               .Create()
               .AddSources(source)
               .WithAnalyzer<Rsa2004>()
               .GenerateAsync();

            // Then
            result
               .AnalyzerResults[typeof(Rsa2004)]
               .Diagnostics
               .Should()
               .NotContain(diagnostic => diagnostic.Id == RSA2004.Id, "only one rule owns a rank component");
        }
    }

    [Theory]
    [InlineData(nameof(DesignTestData.Correct), DesignTestData.Correct)]
    [InlineData(nameof(DesignTestData.InternalBeforePublic), DesignTestData.InternalBeforePublic)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2004>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    [Fact]
    public async Task GivenOtherShapes_WhenAnalyze_ThenNoDiagnosticsReported()
    {
        // Given. Records, structs, generics, nested types, partial types, attributed members and
        // interfaces. The ordering samples only ever exercise a flat class, so these cover the
        // shapes MemberRank must also rank correctly.
        foreach (var source in DesignTestData.CorrectShapes)
        {
            // When
            var result = await GeneratorTestContextBuilder
               .Create()
               .AddSources(source)
               .WithAnalyzer<Rsa2004>()
               .GenerateAsync();

            // Then
            result
               .AnalyzerResults[typeof(Rsa2004)]
               .Diagnostics
               .Should()
               .NotContain(diagnostic => diagnostic.Id == RSA2004.Id);
        }
    }
}
