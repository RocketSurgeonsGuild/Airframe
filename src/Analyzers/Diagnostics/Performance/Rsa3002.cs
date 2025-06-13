using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Performance;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA3002"/>.
/// </summary>
public class Rsa3002 : Rsa3000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA3002];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (((CSharpParseOptions)context.Node.SyntaxTree.Options).LanguageVersion < LanguageVersion.CSharp9)
        {
            return;
        }

        var lambdaExpression = (LambdaExpressionSyntax)context.Node;

        if (lambdaExpression.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return;
        }

        if (lambdaExpression.AttributeLists.Count > 0)
        {
            return;
        }

        var semanticModel = context.SemanticModel;

        var operation = semanticModel.GetOperation(lambdaExpression);
        if (operation == null)
        {
            return;
        }

        if (ContainsThisOrBase(lambdaExpression))
        {
            return;
        }

        // Now do the more expensive data flow analysis
        var dataFlowAnalysis = ModelExtensions.AnalyzeDataFlow(semanticModel, lambdaExpression);
        if (dataFlowAnalysis is not { Succeeded: true })
        {
            return;
        }

        if (dataFlowAnalysis.CapturedInside.Any() || dataFlowAnalysis.CapturedOutside.Any(CanBeCapturedOutside))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA3002, lambdaExpression.GetLocation()));
        bool CanBeCapturedOutside(
            ISymbol symbol) => symbol is not IParameterSymbol &&
            symbol is not IFieldSymbol { IsStatic: true, IsReadOnly: true } &&
            symbol is not IPropertySymbol { IsStatic: true };
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.ParenthesizedLambdaExpression, SyntaxKind.SimpleLambdaExpression];

    private static bool ContainsThisOrBase(SyntaxNode node) => node.DescendantNodes()
       .Any(syntaxNode => syntaxNode.IsKind(SyntaxKind.ThisExpression) || syntaxNode.IsKind(SyntaxKind.BaseExpression));
}