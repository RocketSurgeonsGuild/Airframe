using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA1003"/>.
/// </summary>
public class Rsa1003 : Rsa1000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA1003];

    /// <inheritdoc />
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (!context.Node.Parent.IsKind(SyntaxKind.EqualsValueClause))
        {
            return;
        }

        var invocationExpression = (InvocationExpressionSyntax)context.Node;

        if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            return;
        }

        if (memberAccessExpressionSyntax.Name.Identifier.Text != "ToProperty" || memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
        {
            return;
        }

        var tokens =
            invocationExpression
               .ArgumentList
               .Arguments
               .Select(argument => argument.DescendantNodesAndTokens())
               .SelectMany(token => token.Where(x => x.IsKind(SyntaxKind.SimpleLambdaExpression)))
               .ToList();

        if (!tokens.Any(x => x.IsKind(SyntaxKind.SimpleLambdaExpression)))
        {
            return;
        }

        foreach (var diagnostic in tokens.Select(token => Diagnostic.Create(RSA1003, token.GetLocation(), token)))
        {
            context.ReportDiagnostic(diagnostic);
        }
    }
}