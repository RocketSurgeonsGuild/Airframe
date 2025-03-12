using Rocket.Surgery.Airframe.Defaults.Diagnostics;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

internal abstract class DefaultsSourceData
{
    protected static GeneratorTestContextBuilder DefaultBuilder() => GeneratorTestContextBuilder
       .Create()
       .WithGenerator<DefaultsGenerator>()
       .IgnoreOutputFile("DefaultsAttribute.g.cs");

    protected static GeneratorTestContextBuilder Rsda0001() => GeneratorTestContextBuilder
       .Create()
       .WithAnalyzer<Rsad0001>();
}