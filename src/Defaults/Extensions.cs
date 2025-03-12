using System.Linq;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults;

internal static class Extensions
{
    public static bool HasAccessibility(this IMethodSymbol symbol, params Accessibility[] accessibility) => accessibility.Contains(symbol.DeclaredAccessibility);
}