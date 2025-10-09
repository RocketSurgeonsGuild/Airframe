using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults.Property;

internal partial class DefaultsGenerator
{
    private static INamedTypeSymbol? GetClassForFixture(GeneratorAttributeSyntaxContext syntaxContext)
    {
        var targetSymbol = syntaxContext.TargetSymbol as INamedTypeSymbol;

        if (syntaxContext.Attributes[0].ConstructorArguments.Length == 0)
        {
            return targetSymbol;
        }

        if (syntaxContext.Attributes[0].ConstructorArguments[0].Value is INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol;
        }

        return null;
    }
}