using Rocket.Surgery.Airframe.Defaults.Factory;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Factory;

public class FactoryGeneratorTests
{
    [Fact]
    public async Task GivenAReferenceType_WhenGenerate_ThenGeneratesDefaultProperty()
    {
        // Given, When
        var result =
            await GeneratorTestContextBuilder.Create()
               .WithGenerator<FactoryGenerator>()
               .AddSources(ValueTypeThing)
               .IgnoreOutputFile("FactoryAttribute.g.cs")
               .AddReferences(typeof(Scriban.Template))
               .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters();
    }

    //lang=csharp
    private const string ValueTypeThing = """
        using Rocket.Surgery.Airframe.Defaults;

        [Factory]
        public partial class ValueTypeThing
        {
            public ValueTypeThing(int value) => Value = value;

            public int Value { get; }
        }
        """;
}