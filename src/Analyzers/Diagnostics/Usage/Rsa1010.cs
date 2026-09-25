using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA1010"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa1010 : Rsa1000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA1010];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context) => AnalyzeInvocation(context);

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);

        if (symbolInfo.Symbol is not IMethodSymbol method)
        {
            return;
        }

        // Handle extension methods
        var actualMethod = method.ReducedFrom ?? method;

        if (actualMethod.Name != "Bind" || !IsDynamicDataMethod(actualMethod))
        {
            return;
        }

        if (HasPrecedingObserveOn(invocation))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA1010, GetMethodNameLocation(invocation), actualMethod.Name));
    }

    /// <summary>
    /// Walks outward from the <c>Bind</c> invocation through each earlier call in the fluent
    /// chain, looking for an <c>ObserveOn</c> call anywhere before it.
    /// </summary>
    private static bool HasPrecedingObserveOn(InvocationExpressionSyntax invocation)
    {
        var current = invocation.Expression;

        while (current is MemberAccessExpressionSyntax memberAccess)
        {
            if (memberAccess.Expression is not InvocationExpressionSyntax innerInvocation)
            {
                return false;
            }

            if (innerInvocation.Expression is MemberAccessExpressionSyntax innerMemberAccess &&
                innerMemberAccess.Name.Identifier.Text == "ObserveOn")
            {
                return true;
            }

            current = innerInvocation.Expression;
        }

        return false;
    }

    private static bool IsDynamicDataMethod(IMethodSymbol method)
    {
        if (!method.IsExtensionMethod)
        {
            return false;
        }

        var containingNamespace = method.ContainingNamespace?.ToDisplayString();

        return containingNamespace?.StartsWith("DynamicData") == true ||
               method.ContainingAssembly.Name.Contains("DynamicData");
    }

    private static Location GetMethodNameLocation(InvocationExpressionSyntax invocation) => invocation.Expression switch
    {
        MemberAccessExpressionSyntax memberAccess => memberAccess.Name.GetLocation(),
        IdentifierNameSyntax identifier => identifier.GetLocation(),
        var _ => invocation.GetLocation(),
    };
}
