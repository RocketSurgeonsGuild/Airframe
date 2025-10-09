using Rocket.Surgery.Airframe.Defaults.Diagnostics;
using Rocket.Surgery.Airframe.Defaults.Property;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

internal abstract class DefaultsSourceData
{
    protected static GeneratorTestContextBuilder DefaultBuilder() => GeneratorTestContextBuilder
       .Create()
       .WithGenerator<DefaultsGenerator>()
       .IgnoreOutputFile("DefaultsAttribute.g.cs");

    protected static GeneratorTestContextBuilder Rsad0001() => GeneratorTestContextBuilder
       .Create()
       .WithAnalyzer<Rsad0001>();

    protected static GeneratorTestContextBuilder Rsad0002() => GeneratorTestContextBuilder
       .Create()
       .WithAnalyzer<Rsad0002>();
}