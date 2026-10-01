using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Airframe.Analyzers;

/// <summary>
/// Shared symbol checks for diagnostics that reason about DynamicData's fluent API, so no single
/// rule's band owns a cross-band dependency and no two rules maintain their own copy of the same
/// check.
/// </summary>
internal static class DynamicDataSymbols
{
    /// <summary>
    /// Determines whether <paramref name="method"/> is an extension method declared by DynamicData
    /// (by namespace, falling back to containing assembly for extension methods DynamicData declares
    /// outside its own namespace).
    /// </summary>
    /// <param name="method">The method symbol to check.</param>
    /// <returns><see langword="true"/> when the method is a DynamicData extension method.</returns>
    public static bool IsDynamicDataMethod(IMethodSymbol method)
    {
        if (!method.IsExtensionMethod)
        {
            return false;
        }

        var containingNamespace = method.ContainingNamespace?.ToDisplayString();

        return containingNamespace is "DynamicData" || containingNamespace?.StartsWith("DynamicData.", StringComparison.Ordinal) == true ||
               method.ContainingAssembly?.Name.Contains("DynamicData") == true;
    }

    /// <summary>
    /// Determines whether <paramref name="invocation"/> is a call to DynamicData's <c>AutoRefresh</c>
    /// extension method. This is the single ownership check for "is this call RSA3004's to report
    /// on" -- other rules whose own surface would otherwise overlap AutoRefresh (e.g. RSA1005's
    /// missing-scheduler check) defer to this rather than re-testing the method name themselves.
    /// </summary>
    /// <param name="invocation">The invocation expression.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="method">The resolved method symbol when the invocation is an AutoRefresh call.</param>
    /// <returns><see langword="true"/> when the invocation is an AutoRefresh call.</returns>
    public static bool TryGetAutoRefreshMethod(InvocationExpressionSyntax invocation, SemanticModel semanticModel, out IMethodSymbol method)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(invocation);

        if (symbolInfo.Symbol is not IMethodSymbol methodSymbol)
        {
            method = null!;
            return false;
        }

        var actualMethod = methodSymbol.ReducedFrom ?? methodSymbol;

        if (actualMethod.Name != "AutoRefresh" || !IsDynamicDataMethod(actualMethod))
        {
            method = null!;
            return false;
        }

        method = actualMethod;
        return true;
    }
}
