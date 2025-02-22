using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests;

internal abstract class DefaultsSourceData
{
    protected static GeneratorTestContextBuilder DefaultBuilder() => GeneratorTestContextBuilder
       .Create()
       .WithGenerator<DefaultsGenerator>()
       .IgnoreOutputFile("DefaultsAttribute.g.cs");
}