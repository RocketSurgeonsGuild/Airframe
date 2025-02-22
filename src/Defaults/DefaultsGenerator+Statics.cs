using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults;

public partial class DefaultsGenerator
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

    private static bool Report0001(INamedTypeSymbol classForFixture, SourceProductionContext productionContext)
    {
        if (classForFixture.Constructors.Any(methodSymbol => methodSymbol.HasAccessibility(Accessibility.Private, Accessibility.Protected, Accessibility.Internal)))
        {
            ReportDiagnostic(productionContext, Diagnostics.Defaults0001, classForFixture.Locations);
            return true;
        }

        return false;
    }

    private static void ReportDiagnostic(
        SourceProductionContext productionContext,
        DiagnosticDescriptor diagnosticDescriptor,
        IEnumerable<Location> locations) => ReportDiagnostic(productionContext, diagnosticDescriptor, locations.ToArray());

    private static void ReportDiagnostic(SourceProductionContext productionContext, DiagnosticDescriptor diagnosticDescriptor, params Location[] locations)
    {
        foreach (var location in locations)
        {
            productionContext.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, location));
        }
    }
}