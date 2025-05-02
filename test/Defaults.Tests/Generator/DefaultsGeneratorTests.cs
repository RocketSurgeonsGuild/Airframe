using Rocket.Surgery.Airframe.Defaults.Tests.Data;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Generator;

public class DefaultsGeneratorTests
{
    [Theory]
    // [MemberData(nameof(SimpleReferenceTypeData.Data), MemberType = typeof(SimpleReferenceTypeData))]
    [MemberData(nameof(PrimitivePropertyReferenceTypeData.Data), MemberType = typeof(PrimitivePropertyReferenceTypeData))]
    [MemberData(nameof(ReferencePropertyData.Data), MemberType = typeof(ReferencePropertyData))]
    public async Task GivenAReferenceType_WhenGenerate_ThenGeneratesDefaultProperty(GeneratorTestContext context)
    {
        // Given, When
        var result = await context.GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(context.Id);
    }

    [Theory]
    [MemberData(nameof(NoAttributeData.Data), MemberType = typeof(NoAttributeData))]
    public async Task GivenAReferenceType_WhenGenerate_ThenDoesNotGenerateDefaultProperty(GeneratorTestContext context)
    {
        // Given, When
        var result = await context.GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(context.Id);
    }

    [Theory]
    [MemberData(nameof(SimpleReferenceTypeData.Data), MemberType = typeof(SimpleReferenceTypeData))]
    public async Task Given_When_Then(GeneratorTestContext context)
    {
        // Given, When
        var result = await context.GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(context.Id);
    }
}