using FluentAssertions;
using Rocket.Surgery.Airframe.Defaults.Diagnostics;
using Rocket.Surgery.Airframe.Defaults.Tests.Data;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Diagnostics;

public class Rsad0002Tests
{
    [Theory]
    [MemberData(nameof(MultipleConstructorData.Data), MemberType = typeof(MultipleConstructorData))]
    public async Task Given_When_Then(GeneratorTestContext context)
    {
        // Given,  When
        var result = await context.GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(context.Id).DisableRequireUniquePrefix();
    }

    [Theory]
    [MemberData(nameof(AccessibleConstructorData.Data), MemberType = typeof(AccessibleConstructorData))]
    public async Task GivenClassSingleConstructor_WhenGenerate_ThenNoDiagnosticReported(GeneratorTestContext context)
    {
        // Given, When
        var result = await context.GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == DiagnosticDescriptions.Rsad0002.Id));
    }

    [Theory]
    [MemberData(nameof(MultipleConstructorData.Data), MemberType = typeof(MultipleConstructorData))]
    public async Task GivenClassWithMultipleConstructor_WhenGenerate_ThenDiagnosticReported(GeneratorTestContext context)
    {
        // Given, When
        var result = await context.GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == DiagnosticDescriptions.Rsad0002.Id));
    }
}