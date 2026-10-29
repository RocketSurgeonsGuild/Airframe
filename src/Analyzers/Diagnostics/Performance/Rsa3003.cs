using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA3003"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa3003 : Rsa3000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA3003];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
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

        // Data flow analysis reports the implicit capture of 'this' alongside any local variable
        // or parameter capture, so a single check covers both explicit and implicit closures.
        var dataFlowAnalysis = ModelExtensions.AnalyzeDataFlow(semanticModel, lambdaExpression);
        if (dataFlowAnalysis is not { Succeeded: true })
        {
            return;
        }

        if (!dataFlowAnalysis.CapturedInside.Any() && !dataFlowAnalysis.CapturedOutside.Any())
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA3003, lambdaExpression.GetLocation()));
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.ParenthesizedLambdaExpression, SyntaxKind.SimpleLambdaExpression];
}
