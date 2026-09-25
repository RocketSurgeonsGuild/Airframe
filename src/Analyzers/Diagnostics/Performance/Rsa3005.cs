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

        foreach (var priorInvocation in GetChainInvocations(invocation))
        {
            if (!Rsa3004.TryGetAutoRefreshMethod(priorInvocation, context.SemanticModel, out var priorMethod))
            {
                continue;
            }

            var priorPropertyPath = GetSelectorPropertyPath(priorInvocation, priorMethod, context.SemanticModel);
            if (priorPropertyPath != null && priorPropertyPath == propertyPath)
            {
                context.ReportDiagnostic(Diagnostic.Create(RSA3005, GetMethodNameLocation(invocation)));
                return;
            }
        }
    }

    /// <summary>
    /// Walk the fluent chain an invocation is part of, yielding every invocation earlier in the same chain.
    /// </summary>
    /// <param name="invocation">The invocation to walk backward from.</param>
    /// <returns>The invocations found earlier in the same fluent chain, closest first.</returns>
    private static IEnumerable<InvocationExpressionSyntax> GetChainInvocations(InvocationExpressionSyntax invocation)
    {
        SyntaxNode? current = invocation.Expression;

        while (current != null)
        {
            switch (current)
            {
                case MemberAccessExpressionSyntax memberAccess:
                    current = memberAccess.Expression;
                    break;
                case InvocationExpressionSyntax innerInvocation:
                    yield return innerInvocation;
                    current = innerInvocation.Expression;
                    break;
                default:
                    current = null;
                    break;
            }
        }
    }

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
