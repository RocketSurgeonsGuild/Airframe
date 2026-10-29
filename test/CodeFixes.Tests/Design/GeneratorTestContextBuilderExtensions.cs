using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

internal static class GeneratorTestContextBuilderExtensions
{
    internal static GeneratorTestContextBuilder AddNormalizedSources(
        this GeneratorTestContextBuilder builder, params string[] sources) =>
        builder.AddSources(sources.Select(s => s.ReplaceLineEndings("\n")).ToArray());
}
