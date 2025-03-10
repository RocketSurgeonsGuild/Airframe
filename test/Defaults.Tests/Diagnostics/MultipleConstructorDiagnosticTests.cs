using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Rocket.Surgery.Airframe.Defaults.Diagnostics;
using Rocket.Surgery.Airframe.Defaults.Tests.Data;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Diagnostics;

public class MultipleConstructorDiagnosticTests
{
    [Fact]
    public void Given_When_Then()
    {
        // Given
        var context = GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<MultipleConstructorDiagnostic>()
           .Build();

        // When

        // Then
    }

    [Theory]
    [MemberData(nameof(AccessibleConstructorData.Data), MemberType = typeof(AccessibleConstructorData))]
    public async Task GivenClassWithAccessibleConstructor_WhenGenerate_ThenNoDiagnosticReported(GeneratorTestContext context)
    {
        // Given, When
        var result = await context.GenerateAsync();

        // Then
        result
           .Results
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == DiagnosticDescriptions.Rsad0001.Id));
    }

    [Theory]
    // [MemberData(nameof(InaccessibleConstructorData.Data), MemberType = typeof(InaccessibleConstructorData))]
    [MemberData(nameof(MultipleConstructorData.Data), MemberType = typeof(MultipleConstructorData))]
    public async Task GivenClassWithInaccessibleConstructor_WhenGenerate_ThenDiagnosticReported(GeneratorTestContext context)
    {
        // Given, When
        var result = await context.GenerateAsync();

        // Then
        result
           .Results
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == DiagnosticDescriptions.Rsad0001.Id));
    }
}