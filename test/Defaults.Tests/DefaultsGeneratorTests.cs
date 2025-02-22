using Rocket.Surgery.Airframe.Defaults.Tests.Data;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Defaults.Tests;

public partial class DefaultsGeneratorTests
{
    [Fact]
    public async Task GivenAGenerator_WhenGenerate_ThenAttributeGenerated()
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
    public async Task GivenAReferenceType_WhenGenerate_ThenGeneratesDefaultProperty(GeneratorTestContext context)
    {
        // Given, When
        var result = await context.GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(context.Id);
    }
}