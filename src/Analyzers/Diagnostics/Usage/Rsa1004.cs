using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA1004"/>.
/// </summary>
public class Rsa1004 : Rsa1000
{
    /// <inheritdoc />
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocationExpression = (InvocationExpressionSyntax)context.Node;

        if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            return;
        }

        if (memberAccessExpressionSyntax.Name.Identifier.Text != "WhenAnyValue"
         || memberAccessExpressionSyntax.Expression is not ThisExpressionSyntax)
        {
            return;
        }

        var diagnostics =
            invocationExpression
               .ArgumentList
               .Arguments
               .Select(argumentSyntax => argumentSyntax.Expression)
               .OfType<SimpleLambdaExpressionSyntax>()
               .Where(simpleLambdaExpressionSyntax => !simpleLambdaExpressionSyntax.ExpressionBody.IsKind(SyntaxKind.SimpleMemberAccessExpression))
               .Select(simpleLambdaExpressionSyntax => simpleLambdaExpressionSyntax.GetLastToken())
               .Select(syntaxToken => Diagnostic.Create(RSA1004, syntaxToken.GetLocation(), syntaxToken))
               .ToList();

        foreach (var diagnostic in diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA1004];
}