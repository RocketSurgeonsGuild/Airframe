using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers;

/// <summary>
/// Represents <see cref="Descriptions.RSA1001"/>.
/// </summary>
public class Rsa1001 : Rsa1000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA1001];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocationExpression = (InvocationExpressionSyntax)context.Node;

        if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            return;
        }

        if (memberAccessExpressionSyntax.Name.Identifier.Text != "InvokeCommand"
         || memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
        {
            return;
        }

        foreach (var syntaxTokens in invocationExpression.ArgumentList.Arguments.Select(argument => argument.ChildNodesAndTokens()))
        {
            var diagnostics =
                syntaxTokens
                   .Where(token => !token.IsKind(SyntaxKind.ThisExpression)
                     && !token.IsKind(SyntaxKind.SimpleLambdaExpression))
                   .Select(token => Diagnostic.Create(RSA1001, token.GetLocation(), token));

            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}