using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Airframe.Analyzers;

/// <summary>
/// Shared symbol checks for <c>System.Reactive.Linq.Observable.FromEvent</c> and
/// <c>System.Reactive.Linq.Observable.FromEventPattern</c>, so the RSA3006 analyzer and its code fix
/// agree on exactly which calls are in scope.
/// </summary>
internal static class ObservableFromEventSymbols
{
    /// <summary>
    /// The name of <c>Observable.FromEvent</c>.
    /// </summary>
    public const string FromEvent = nameof(FromEvent);

    /// <summary>
    /// The name of <c>Observable.FromEventPattern</c>.
    /// </summary>
    public const string FromEventPattern = nameof(FromEventPattern);

    /// <summary>
    /// Cheap syntactic pre-check: does <paramref name="invocation"/> call something named
    /// <c>FromEvent</c> or <c>FromEventPattern</c>? Used before touching the semantic model.
    /// </summary>
    /// <param name="invocation">The invocation expression.</param>
    /// <returns><see langword="true"/> when the invoked name matches.</returns>
    public static bool HasFromEventName(InvocationExpressionSyntax invocation)
    {
        var name = invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name,
            SimpleNameSyntax simpleName => simpleName,
            var _ => null,
        };

        return name?.Identifier.ValueText is FromEvent or FromEventPattern;
    }

    /// <summary>
    /// Resolves <paramref name="invocation"/> to <c>Observable.FromEvent</c> or <c>Observable.FromEventPattern</c>.
    /// </summary>
    /// <param name="invocation">The invocation expression.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="method">The resolved method symbol when the invocation is in scope.</param>
    /// <returns><see langword="true"/> when the invocation calls one of the Rx event bridges.</returns>
    public static bool TryGetFromEventMethod(InvocationExpressionSyntax invocation, SemanticModel semanticModel, out IMethodSymbol method)
    {
        if (!HasFromEventName(invocation) ||
            semanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol ||
            !IsFromEventMethod(methodSymbol))
        {
            method = null!;
            return false;
        }

        method = methodSymbol;
        return true;
    }

    /// <summary>
    /// Determines whether <paramref name="method"/> is <c>System.Reactive.Linq.Observable.FromEvent</c> or
    /// <c>System.Reactive.Linq.Observable.FromEventPattern</c>.
    /// </summary>
    /// <param name="method">The method symbol to check.</param>
    /// <returns><see langword="true"/> when the method is one of the Rx event bridges.</returns>
    public static bool IsFromEventMethod(IMethodSymbol method) =>
        method is { IsStatic: true, Name: FromEvent or FromEventPattern, ContainingType: { Name: "Observable" } containingType } &&
        containingType.ContainingNamespace?.ToDisplayString() == "System.Reactive.Linq";

    /// <summary>
    /// Determines whether <paramref name="method"/> takes an <c>IScheduler</c>, which changes where the handler is
    /// added and removed and therefore has no direct <c>Observable.Create</c> equivalent.
    /// </summary>
    /// <param name="method">The method symbol to check.</param>
    /// <returns><see langword="true"/> when one of the parameters is an <c>IScheduler</c>.</returns>
    public static bool HasScheduler(IMethodSymbol method) =>
        method.Parameters.Any(parameter =>
            parameter.Type is { Name: "IScheduler" } type &&
            type.ContainingNamespace?.ToDisplayString() == "System.Reactive.Concurrency");

    /// <summary>
    /// Determines whether <paramref name="method"/> is one of the reflection overloads that locate the event by name.
    /// </summary>
    /// <param name="method">The method symbol to check.</param>
    /// <returns><see langword="true"/> when the method takes an event name.</returns>
    public static bool IsReflectionOverload(IMethodSymbol method) =>
        method.Parameters.Any(parameter => parameter.Type.SpecialType == SpecialType.System_String);
}
