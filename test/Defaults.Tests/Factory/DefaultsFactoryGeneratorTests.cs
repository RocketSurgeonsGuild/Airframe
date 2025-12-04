using Rocket.Surgery.Airframe.Defaults.Property;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Factory;

public class DefaultsFactoryGeneratorTests
{
    [Fact]
    public async Task GivenAReferenceType_WhenGenerate_ThenGeneratesDefaultProperty()
    {
        // Given, When
        var result = await GeneratorTestContextBuilder.Create().WithGenerator<DefaultsGenerator>().AddSources(ValueTypeThing).GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters();
    }

    //lang=csharp
    private const string ValueTypeThing = """
        public partial class ValueTypeThing
        {
            public ValueTypeThing(int value) => Value = value;

            public int Value { get; }
        }
        """;
}