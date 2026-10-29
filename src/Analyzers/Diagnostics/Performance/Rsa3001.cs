using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA3001"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa3001 : Rsa3000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA3001];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (context.Node.Parent is not ExpressionStatementSyntax expressionStatementSyntax)
        {
            return;
        }

        if (expressionStatementSyntax.Expression is not InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return;
        }

        if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            return;
        }

        if (!_subscriptionAccess.Contains(memberAccessExpressionSyntax.Name.Identifier.Text) ||
            memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA3001, memberAccessExpressionSyntax.Name.Identifier.GetLocation()));
    }

    // Exact match only: a substring check here (e.g. matching "Value" against "AsValue") would
    // false-positive on unrelated fluent calls that happen to share a substring with one of these.
    private readonly HashSet<string> _subscriptionAccess = ["InvokeCommand", "HandledSubscribe", "SafeSubscribe", "SubscribeSafe", "Subscribe", "ToProperty", "BindTo", "AsValue"];
}