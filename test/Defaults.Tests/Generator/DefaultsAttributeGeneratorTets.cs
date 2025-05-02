using Rocket.Surgery.Airframe.Defaults.Generator;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Generator;

public class DefaultsAttributeGeneratorTets
{
    [Fact]
    public async Task GivenAGenerator_WhenGenerate_ThenAttributeGenerated()
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddReferences(typeof(List<>))
           .Build()
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).ScrubLines(text => text.Contains("System.CodeDom.Compiler.GeneratedCode"));
    }
}