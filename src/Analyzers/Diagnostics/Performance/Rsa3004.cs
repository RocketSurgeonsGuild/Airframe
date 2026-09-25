using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA3004"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa3004 : Rsa3000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA3004];

    /// <summary>
    /// Determine whether the given invocation is a call to DynamicData's <c>AutoRefresh</c> extension method.
    /// </summary>
    /// <param name="invocation">The invocation expression.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="method">The resolved method symbol when the invocation is an AutoRefresh call.</param>
    /// <returns><see langword="true"/> when the invocation is an AutoRefresh call.</returns>
    internal static bool TryGetAutoRefreshMethod(InvocationExpressionSyntax invocation, SemanticModel semanticModel, out IMethodSymbol method)
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

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (!TryGetAutoRefreshMethod(invocation, context.SemanticModel, out var method))
        {
            return;
        }

        var schedulerParameter = method.Parameters.FirstOrDefault(parameter => IsSchedulerType(parameter.Type));
        if (schedulerParameter is not { IsOptional: true })
        {
            return;
        }

        var isProvided = false;
        if (context.SemanticModel.GetOperation(invocation) is IInvocationOperation operation)
        {
            isProvided = operation.Arguments.Any(argument =>
                argument.Parameter?.OriginalDefinition.Equals(schedulerParameter.OriginalDefinition, SymbolEqualityComparer.Default) == true &&
                argument.ArgumentKind != ArgumentKind.DefaultValue);
        }

        if (isProvided)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA3004, GetMethodNameLocation(invocation)));
    }

    private static bool IsDynamicDataMethod(IMethodSymbol method)
    {
        var containingNamespace = method.ContainingNamespace?.ToDisplayString();

        return containingNamespace?.StartsWith("DynamicData") == true ||
               method.ContainingAssembly.Name.Contains("DynamicData");
    }

    private static bool IsSchedulerType(ITypeSymbol? type)
    {
        if (type == null)
        {
            return false;
        }

        if (type is INamedTypeSymbol namedType && namedType.IsGenericType && namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
        {
            return IsSchedulerType(namedType.TypeArguments[0]);
        }

        if (type.Name == "IScheduler" &&
            (type.ContainingNamespace?.ToDisplayString() == "System.Reactive.Concurrency" ||
             type.ContainingNamespace?.ToDisplayString() == "System.Reactive"))
        {
            return true;
        }

        return type.AllInterfaces.Any(i =>
            i.Name == "IScheduler" &&
            (i.ContainingNamespace?.ToDisplayString() == "System.Reactive.Concurrency" ||
             i.ContainingNamespace?.ToDisplayString() == "System.Reactive"));
    }

    private static Location GetMethodNameLocation(InvocationExpressionSyntax invocation) => invocation.Expression switch
    {
        MemberAccessExpressionSyntax memberAccess => memberAccess.Name.GetLocation(),
        IdentifierNameSyntax identifier => identifier.GetLocation(),
        var _ => invocation.GetLocation(),
    };
}
