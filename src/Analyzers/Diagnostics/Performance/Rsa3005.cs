using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA3005"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa3005 : Rsa3000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA3005];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (!Rsa3004.TryGetAutoRefreshMethod(invocation, context.SemanticModel, out var method))
        {
            return;
        }

        var propertyPath = GetSelectorPropertyPath(invocation, method, context.SemanticModel);
        if (propertyPath == null)
        {
            return;
        }

        foreach (var candidate in CollectConnectedAutoRefreshInvocations(invocation, context.SemanticModel))
        {
            // Only compare against invocations earlier in the tree. Every other AutoRefresh call reachable
            // from this one is reachable symmetrically (the graph below is walked in both directions), so
            // comparing only against earlier ones is what keeps a single duplicate pair from being reported
            // twice (once per direction).
            //
            // "Earlier" is measured by the end of the invocation's span (where its closing parenthesis is),
            // not its start: every invocation in a fluent chain shares the same start position (the leftmost
            // receiver token, e.g. `cache` in `cache.Connect().AutoRefresh(...).AutoRefresh(...)`), so start
            // position cannot distinguish an inner (earlier-written) call from an outer (later-written) one -
            // only their end positions differ.
            if (candidate.Span.End >= invocation.Span.End)
            {
                continue;
            }

            if (!Rsa3004.TryGetAutoRefreshMethod(candidate, context.SemanticModel, out var candidateMethod))
            {
                continue;
            }

            var candidatePropertyPath = GetSelectorPropertyPath(candidate, candidateMethod, context.SemanticModel);
            if (candidatePropertyPath != null && candidatePropertyPath == propertyPath)
            {
                context.ReportDiagnostic(Diagnostic.Create(RSA3005, GetMethodNameLocation(invocation)));
                return;
            }
        }
    }

    /// <summary>
    /// Find every AutoRefresh invocation reachable from <paramref name="invocation"/> through the pipeline it
    /// is being built as part of, "regardless of the complexity" of that pipeline: a linear fluent chain, an
    /// Rx combinator (<c>Merge</c>, <c>CombineLatest</c>, <c>Zip</c>, <c>Concat</c>, <c>Switch</c>, ...) whose
    /// arguments are separate source pipelines, and/or a local variable that is built in one statement and
    /// consumed by a combinator in a later one.
    /// </summary>
    /// <remarks>
    /// This walks a small graph of "pipeline regions":
    /// <list type="bullet">
    /// <item>The starting region is the nearest expression-composing boundary around <paramref name="invocation"/>
    /// (see <see cref="GetPipelineRoot"/>) — this alone covers the linear-chain case and the inline-combinator
    /// case, since both keep every relevant AutoRefresh call inside one statement-level expression.</item>
    /// <item>Whenever a region references a local variable (either because the region <em>is</em> that
    /// variable's own initializer, or because it contains an identifier that reads the variable), the graph
    /// also links to that variable's declaration (to pick up its own initializer, "backward") and to every
    /// other place that variable is referenced within its declaring block ("forward") — so a variable built in
    /// one statement and merged with another in a later one is still connected.</item>
    /// </list>
    /// A local's declaration and forward references are resolved via the semantic model (<see cref="ILocalSymbol"/>),
    /// so this only follows actual data flow — it never widens to "everything else in the block" the way a
    /// blind scope-widen would, which is what keeps two independent AutoRefresh pipelines that are never
    /// combined from being conflated (see the "unrelated pipelines" test case).
    /// <para>
    /// Known boundary: a variable assigned across multiple branches (e.g. set in an <c>if</c>/<c>else</c>
    /// rather than declared with an initializer) or a method parameter with no visible initializer is not
    /// resolved — those cases fall out of the "region is itself an initializer" / "identifier resolves to a
    /// local with a declarator initializer" checks below and are silently left unlinked. False negatives here
    /// are the intended, safer failure mode (consistent with the rest of this analyzer).
    /// </para>
    /// </remarks>
    /// <param name="invocation">The AutoRefresh invocation to find connected AutoRefresh invocations for.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <returns>Every AutoRefresh invocation reachable from <paramref name="invocation"/>, itself included.</returns>
    private static IEnumerable<InvocationExpressionSyntax> CollectConnectedAutoRefreshInvocations(
        InvocationExpressionSyntax invocation,
        SemanticModel semanticModel)
    {
        var visitedRegions = new HashSet<SyntaxNode>();
        var visitedLocals = new HashSet<ILocalSymbol>(SymbolEqualityComparer.Default);
        var regionsToVisit = new Queue<SyntaxNode>();
        var results = new List<InvocationExpressionSyntax>();

        regionsToVisit.Enqueue(GetPipelineRoot(invocation));

        while (regionsToVisit.Count > 0)
        {
            var region = regionsToVisit.Dequeue();
            if (!visitedRegions.Add(region))
            {
                continue;
            }

            foreach (var candidate in region.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
            {
                if (Rsa3004.TryGetAutoRefreshMethod(candidate, semanticModel, out _))
                {
                    results.Add(candidate);
                }
            }

            // This region is itself a local variable's initializer (e.g. `var a = cache.Connect().AutoRefresh(...)`)
            // - link forward to every other place that variable is used, and backward to its own initializer.
            if (region.Parent is EqualsValueClauseSyntax { Parent: VariableDeclaratorSyntax ownDeclarator } &&
                semanticModel.GetDeclaredSymbol(ownDeclarator) is ILocalSymbol ownLocal)
            {
                LinkLocalVariable(ownLocal, ownDeclarator, semanticModel, visitedLocals, regionsToVisit);
            }

            // This region references a local variable declared elsewhere (e.g. `a.Merge(b)`) - link backward to
            // that variable's own initializer, and forward to its other usages.
            foreach (var identifier in region.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>())
            {
                if (identifier.Parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Name == identifier)
                {
                    continue;
                }

                if (semanticModel.GetSymbolInfo(identifier).Symbol is not ILocalSymbol referencedLocal)
                {
                    continue;
                }

                if (referencedLocal.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not VariableDeclaratorSyntax declarator)
                {
                    continue;
                }

                LinkLocalVariable(referencedLocal, declarator, semanticModel, visitedLocals, regionsToVisit);
            }
        }

        return results;
    }

    /// <summary>
    /// Queue up the pipeline regions connected to a local variable: its own initializer ("backward" from a
    /// reference to it) and every other reference to it within its declaring block ("forward" from its
    /// declaration). No-ops (and does not recurse) once a variable has already been linked.
    /// </summary>
    private static void LinkLocalVariable(
        ILocalSymbol local,
        VariableDeclaratorSyntax declarator,
        SemanticModel semanticModel,
        HashSet<ILocalSymbol> visitedLocals,
        Queue<SyntaxNode> regionsToVisit)
    {
        if (!visitedLocals.Add(local))
        {
            return;
        }

        if (declarator.Initializer is { } initializer)
        {
            regionsToVisit.Enqueue(GetPipelineRoot(initializer.Value));
        }

        // A variable declared without an initializer (e.g. assigned across separate `if`/`else` branches) has
        // nothing here to link backward to; its forward usages are still discoverable below.
        var searchScope = declarator.FirstAncestorOrSelf<BlockSyntax>();
        if (searchScope == null)
        {
            return;
        }

        foreach (var reference in searchScope.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>())
        {
            if (reference.Parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Name == reference)
            {
                continue;
            }

            if (SymbolEqualityComparer.Default.Equals(semanticModel.GetSymbolInfo(reference).Symbol, local))
            {
                regionsToVisit.Enqueue(GetPipelineRoot(reference));
            }
        }
    }

    /// <summary>
    /// Walk upward from a node through the syntax kinds that merely compose one larger expression (member
    /// access, invocation, argument lists, parentheses, casts, conditional expressions) to find the nearest
    /// enclosing statement-level or initializer-level boundary - e.g. an <see cref="ExpressionStatementSyntax"/>,
    /// a <see cref="LocalDeclarationStatementSyntax"/>'s <see cref="EqualsValueClauseSyntax"/>, or a
    /// <see cref="ReturnStatementSyntax"/>. The node just inside that boundary is "one pipeline construction".
    /// </summary>
    /// <param name="node">The node to walk upward from.</param>
    /// <returns>The outermost node still inside the same pipeline-construction expression.</returns>
    private static SyntaxNode GetPipelineRoot(SyntaxNode node)
    {
        var current = node;

        while (current.Parent is { } parent && IsPipelineComposingNode(parent))
        {
            current = parent;
        }

        return current;
    }

    private static bool IsPipelineComposingNode(SyntaxNode node) => node switch
    {
        InvocationExpressionSyntax => true,
        MemberAccessExpressionSyntax => true,
        ArgumentSyntax => true,
        ArgumentListSyntax => true,
        ParenthesizedExpressionSyntax => true,
        CastExpressionSyntax => true,
        ConditionalExpressionSyntax => true,
        var _ => false,
    };

    /// <summary>
    /// Locate the selector argument passed to an AutoRefresh call by semantic parameter binding (matching
    /// <see cref="IArgumentOperation.Parameter"/> against the selector parameter), rather than positionally,
    /// so named/reordered arguments (e.g. <c>AutoRefresh(scheduler: ..., propertyAccessor: x =&gt; x.Thing)</c>)
    /// are still resolved. Then extract and normalize its trailing member-access path, ignoring the lambda's
    /// own parameter name, e.g. both <c>x =&gt; x.Foo</c> and <c>y =&gt; y.Foo</c> normalize to <c>Foo</c>.
    /// </summary>
    /// <param name="invocation">The AutoRefresh invocation.</param>
    /// <param name="method">The resolved AutoRefresh method symbol.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <returns>The normalized property path, or <see langword="null"/> when the selector cannot be confidently compared.</returns>
    private static string? GetSelectorPropertyPath(InvocationExpressionSyntax invocation, IMethodSymbol method, SemanticModel semanticModel)
    {
        var selectorParameter = method.Parameters.FirstOrDefault(parameter => IsSelectorType(parameter.Type));
        if (selectorParameter == null)
        {
            return null;
        }

        if (semanticModel.GetOperation(invocation) is not IInvocationOperation operation)
        {
            return null;
        }

        var selectorArgumentOperation = operation.Arguments.FirstOrDefault(argument =>
            argument.Parameter?.OriginalDefinition.Equals(selectorParameter.OriginalDefinition, SymbolEqualityComparer.Default) == true &&
            argument.ArgumentKind != ArgumentKind.DefaultValue);

        if (selectorArgumentOperation?.Syntax is not ArgumentSyntax selectorArgument)
        {
            return null;
        }

        return GetSelectorPropertyPath(selectorArgument);
    }

    /// <summary>
    /// Determine whether a parameter's type is the <c>Expression&lt;Func&lt;TObject, TProperty&gt;&gt;</c> shape
    /// used by AutoRefresh's property-accessor selector.
    /// </summary>
    /// <param name="type">The parameter type.</param>
    /// <returns><see langword="true"/> when the type is an <c>Expression&lt;T&gt;</c>.</returns>
    private static bool IsSelectorType(ITypeSymbol? type) =>
        type is INamedTypeSymbol { Name: "Expression", TypeArguments.Length: 1 } namedType &&
        namedType.ContainingNamespace?.ToDisplayString() == "System.Linq.Expressions";

    private static string? GetSelectorPropertyPath(ArgumentSyntax? selectorArgument)
    {
        var parameterName = selectorArgument?.Expression switch
        {
            SimpleLambdaExpressionSyntax simple => simple.Parameter.Identifier.Text,
            ParenthesizedLambdaExpressionSyntax { ParameterList.Parameters.Count: 1 } parenthesized =>
                parenthesized.ParameterList.Parameters[0].Identifier.Text,
            var _ => null,
        };

        if (parameterName == null || selectorArgument!.Expression is not LambdaExpressionSyntax { ExpressionBody: { } body })
        {
            return null;
        }

        return GetMemberPath(body, parameterName);
    }

    private static string? GetMemberPath(ExpressionSyntax expression, string parameterName)
    {
        var segments = new List<string>();
        ExpressionSyntax current = expression;

        while (current is MemberAccessExpressionSyntax memberAccess)
        {
            segments.Insert(0, memberAccess.Name.Identifier.Text);
            current = memberAccess.Expression;
        }

        if (current is not IdentifierNameSyntax identifier || identifier.Identifier.Text != parameterName || segments.Count == 0)
        {
            return null;
        }

        return string.Join(".", segments);
    }

    private static Location GetMethodNameLocation(InvocationExpressionSyntax invocation) => invocation.Expression switch
    {
        MemberAccessExpressionSyntax memberAccess => memberAccess.Name.GetLocation(),
        IdentifierNameSyntax identifier => identifier.GetLocation(),
        var _ => invocation.GetLocation(),
    };
}
