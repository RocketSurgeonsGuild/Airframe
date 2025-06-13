using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Performance;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA3001"/>.
/// </summary>
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

        if (!_subscriptionAccess.Any(methodName => methodName.Contains(memberAccessExpressionSyntax.Name.Identifier.Text)) ||
            memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA3001, memberAccessExpressionSyntax.Name.Identifier.GetLocation()));
    }

    private readonly List<string> _subscriptionAccess = ["InvokeCommand", "HandledSubscribe", "SafeSubscribe", "SubscribeSafe", "ToProperty", "BindTo"];
}