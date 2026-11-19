using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA3002"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa3002 : Rsa3000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA3002];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var function = (AnonymousFunctionExpressionSyntax)context.Node;
        if (StaticAnonymousFunctions.CanMakeStatic(context.SemanticModel, function, context.CancellationToken))
        {
            context.ReportDiagnostic(Diagnostic.Create(RSA3002, function.GetLocation()));
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() =>
        [SyntaxKind.ParenthesizedLambdaExpression, SyntaxKind.SimpleLambdaExpression, SyntaxKind.AnonymousMethodExpression];
}
