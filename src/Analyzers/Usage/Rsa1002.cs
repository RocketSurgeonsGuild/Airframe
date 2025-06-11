using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers;

/// <summary>
/// Represents <see cref="Descriptions.RSA1002"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa1002 : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.Analyze
          | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(
            action: Analyze,
            syntaxKinds: new[]
            {
                SyntaxKind.InvocationExpression
            });
    }

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA1002, RSA1004];

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocationExpression = (InvocationExpressionSyntax)context.Node;

        if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            return;
        }

        if (memberAccessExpressionSyntax.Name.Identifier.Text != "BindTo" || memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
        {
            return;
        }

        var tokens =
            invocationExpression
               .ArgumentList
               .Arguments
               .Select(argument => argument.DescendantNodesAndTokens())
               .SelectMany(token => token.Where(x => x.IsKind(SyntaxKind.SimpleLambdaExpression) || x.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
               .ToList();

        if (tokens.Any(syntaxNodeOrToken => syntaxNodeOrToken.IsKind(SyntaxKind.SimpleLambdaExpression)) &&
            tokens.Any(syntaxNodeOrToken => syntaxNodeOrToken.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
        {
            return;
        }

        foreach (var diagnostic in tokens.Select(token => Diagnostic.Create(RSA1002, token.GetLocation(), token)))
        {
            context.ReportDiagnostic(diagnostic);
        }
    }
}