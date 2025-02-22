using FluentAssertions;
using Rocket.Surgery.Airframe.Defaults.Tests.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Defaults.Tests;

public partial class DefaultsGeneratorTests
{
    [Fact]
    public void Given_When_Then()
    {
        // Given
        const string NameOf = nameof(Rocket.Surgery.Airframe.Defaults.DefaultsAttribute);

        // When

        // Then
        nameof(global::Rocket.Surgery.Airframe.Defaults.DefaultsAttribute).Should().Be("Rocket.Surgery.Airframe.Defaults.DefaultsAttribute");
    }

    [Fact]
    public async Task GivenAGenerator_WhenGenerate_ThenGeneratesDefaultAttribute()
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithGenerator<DefaultsGenerator>()
           .AddReferences(typeof(List<>))
           .Build()
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).ScrubLines(text => text.Contains("System.CodeDom.Compiler.GeneratedCode"));
    }

    [Theory]
    [MemberData(nameof(SimpleReferenceTypeData.Data), MemberType = typeof(SimpleReferenceTypeData))]
    public async Task Given_WhenGenerate_Then(GeneratorTestContext context)
    {
        // Given, When
        var result = await context.GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(context.Id);
    }
}