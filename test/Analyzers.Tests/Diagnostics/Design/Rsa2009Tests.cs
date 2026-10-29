using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
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

    [Theory]
    [InlineData("Option.Unit.Tests.cs", "OptionUnitTests")]
    [InlineData("ProcessRunner.Integration.Tests.cs", "ProcessRunnerIntegrationTests")]
    [InlineData("Thing{T}.Unit.Tests.cs", "ThingUnitTests")]
    public async Task GivenConventionalTestFileName_WhenAnalyze_ThenNoDiagnosticsReported(string path, string typeName)
    {
        // Given, When
        var diagnostics = await AnalyzeAsync(path, typeName);

        // Then
        diagnostics.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Option.Unit.Tests.cs", "OptionIntegrationTests")]
    [InlineData("Option.Unit.Tests.cs", "Other")]
    [InlineData("Option.Integration.Tests.cs", "OptionUnitTests")]
    public async Task GivenMismatchedTestFileName_WhenAnalyze_ThenDiagnosticReported(string path, string typeName)
    {
        // Given, When
        var diagnostics = await AnalyzeAsync(path, typeName);

        // Then
        diagnostics.Should().ContainSingle();
    }

    private static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(string path, string typeName)
    {
        var tree = CSharpSyntaxTree.ParseText(
            $"namespace Sample {{ public class {typeName} {{ }} }}",
            new CSharpParseOptions(LanguageVersion.Latest),
            path);

        var compilation = CSharpCompilation.Create(
            "Rsa2009Tests",
            [tree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var diagnostics = await compilation
           .WithAnalyzers([new Rsa2009()])
           .GetAnalyzerDiagnosticsAsync();

        return diagnostics.Where(diagnostic => diagnostic.Id == RSA2009.Id).ToImmutableArray();
    }
}
