using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Defaults.Tests;

public class DefaultsGeneratorTests
{
    [Fact]
    public async Task GivenAGenerator_WhenGenerate_ThenGeneratesDefaultAttribute()
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithGenerator<DefaultsGenerator>()
           .AddReferences(typeof(ILogger<>))
           .AddReferences(typeof(List<>))
           .Build()
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).ScrubLines(text => text.Contains("System.CodeDom.Compiler.GeneratedCode"));
    }
}