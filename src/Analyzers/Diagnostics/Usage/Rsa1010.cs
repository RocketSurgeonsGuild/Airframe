using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
    /// <summary>
    /// Upper bound on how far <see cref="GetChainInvocations"/> will walk backward through a
    /// fluent chain (and any variable initializers it follows into). This is a defensive guard
    /// against a pathological or self-referential chain in code that doesn't fully compile
    /// (e.g. mid-edit); no legitimate fluent chain is expected to approach this depth.
    /// </summary>
    private const int MaxChainWalkDepth = 64;

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

        if (HasPrecedingObserveOn(invocation, context.SemanticModel))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA1010, GetMethodNameLocation(invocation), actualMethod.Name));
    }

    /// <summary>
    /// Determines whether an <c>ObserveOn</c> call appears anywhere earlier in the fluent chain
    /// leading into <paramref name="invocation"/> (the <c>Bind</c> call).
    /// </summary>
    private static bool HasPrecedingObserveOn(InvocationExpressionSyntax invocation, SemanticModel semanticModel) =>
        GetChainInvocations(invocation, semanticModel)
           .Any(chainInvocation =>
                chainInvocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.Text == "ObserveOn");

    /// <summary>
    /// Walks backward through a fluent invocation chain starting at <paramref name="invocation"/>,
    /// yielding every invocation encountered (including <paramref name="invocation"/> itself).
    /// </summary>
    /// <remarks>
    /// When the walk bottoms out at a bare identifier (e.g. a local variable), it follows that
    /// identifier to its declaring variable's initializer and keeps walking -- so a chain split
    /// across a statement boundary, such as
    /// <c>var pipeline = source.ObserveOn(x); pipeline.Bind(items);</c>, is still recognized as
    /// having a preceding <c>ObserveOn</c>. This is intentionally not general dataflow/alias
    /// analysis: only a single level of "identifier resolves to a local variable with a simple
    /// initializer" indirection is followed. A variable reassigned after declaration, a value
    /// that arrives via a method parameter, or a value returned from a helper method is not
    /// tracked, so RSA1010 may still (rarely) miss an <c>ObserveOn</c> for those shapes.
    /// </remarks>
    private static IEnumerable<InvocationExpressionSyntax> GetChainInvocations(ExpressionSyntax expression, SemanticModel semanticModel)
    {
        var current = expression;
        var depth = 0;

        while (depth++ < MaxChainWalkDepth)
        {
            switch (current)
            {
                case InvocationExpressionSyntax invocation:
                    yield return invocation;
                    current = invocation.Expression;
                    continue;

                case MemberAccessExpressionSyntax memberAccess:
                    current = memberAccess.Expression;
                    continue;

                case IdentifierNameSyntax identifier when TryGetInitializer(identifier, semanticModel) is { } initializer:
                    current = initializer;
                    continue;

                default:
                    yield break;
            }
        }
    }

    /// <summary>
    /// Resolves an identifier to the initializer expression of the local variable it refers to,
    /// if any.
    /// </summary>
    private static ExpressionSyntax? TryGetInitializer(IdentifierNameSyntax identifier, SemanticModel semanticModel)
    {
        if (semanticModel.GetSymbolInfo(identifier).Symbol is not ILocalSymbol local)
        {
            return null;
        }

        var declaringSyntax = local.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

        return declaringSyntax is VariableDeclaratorSyntax { Initializer.Value: { } initializerValue }
            ? initializerValue
            : null;
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
}
