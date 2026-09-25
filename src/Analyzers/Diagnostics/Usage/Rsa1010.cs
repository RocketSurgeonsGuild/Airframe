using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA1010"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa1010 : Rsa1000
{
    /// <summary>
    /// Upper bound on how many statement-boundary hops <see cref="HasPrecedingObserveOn"/> will
    /// follow (a variable's initializer, then that initializer's own variable, and so on). This
    /// is a defensive guard against a pathological or self-referential chain in code that
    /// doesn't fully compile (e.g. mid-edit); no legitimate fluent chain is expected to approach
    /// this depth.
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
    /// <remarks>
    /// The chain within a single statement is walked via <see cref="Rsa3005.GetChainInvocations"/>
    /// -- shared rather than re-implemented, so RSA1010 and RSA3005 don't maintain two copies of
    /// the same fluent-chain walk. When that walk bottoms out at a bare identifier (e.g. a local
    /// variable), this follows the identifier to its declaring variable's initializer and keeps
    /// walking from there -- so a chain split across a statement boundary, such as
    /// <c>var pipeline = source.ObserveOn(x); pipeline.Bind(items);</c>, is still recognized as
    /// having a preceding <c>ObserveOn</c>. This is intentionally not general dataflow/alias
    /// analysis: only "identifier resolves to a local variable with a simple invocation
    /// initializer" indirection is followed, one statement boundary at a time. A variable
    /// reassigned after declaration, a value that arrives via a method parameter, or a value
    /// returned from a helper method is not tracked, so RSA1010 may still (rarely) miss an
    /// <c>ObserveOn</c> for those shapes.
    /// </remarks>
    private static bool HasPrecedingObserveOn(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
    {
        var current = invocation;

        for (var depth = 0; depth < MaxChainWalkDepth; depth++)
        {
            if (IsObserveOnInvocation(current) || Rsa3005.GetChainInvocations(current).Any(IsObserveOnInvocation))
            {
                return true;
            }

            if (GetChainRoot(current) is not IdentifierNameSyntax identifier ||
                TryGetInitializer(identifier, semanticModel) is not InvocationExpressionSyntax initializerInvocation)
            {
                return false;
            }

            current = initializerInvocation;
        }

        return false;
    }

    private static bool IsObserveOnInvocation(InvocationExpressionSyntax invocation) =>
        invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
        memberAccess.Name.Identifier.Text == "ObserveOn";

    /// <summary>
    /// Finds the expression a fluent chain bottoms out at -- the receiver that isn't itself part
    /// of the member-access/invocation alternation <see cref="Rsa3005.GetChainInvocations"/> walks.
    /// </summary>
    private static ExpressionSyntax GetChainRoot(InvocationExpressionSyntax invocation)
    {
        var current = (ExpressionSyntax)invocation;

        while (true)
        {
            current = current switch
            {
                InvocationExpressionSyntax currentInvocation => currentInvocation.Expression,
                MemberAccessExpressionSyntax memberAccess => memberAccess.Expression,
                var root => root,
            };

            if (current is not (InvocationExpressionSyntax or MemberAccessExpressionSyntax))
            {
                return current;
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
