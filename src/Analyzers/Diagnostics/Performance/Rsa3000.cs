using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.Airframe.Analyzers.Performance;

/// <summary>
/// Represents an RSA 3000 level analyzer.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public abstract class Rsa3000 : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public sealed override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.Analyze
          | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.RegisterSyntaxNodeAction(action: Analyze, syntaxKinds: GetKind());
    }

    /// <summary>
    /// Analyze the <see cref="SyntaxNodeAnalysisContext"/>.
    /// </summary>
    /// <param name="context">The context.</param>
    protected virtual void Analyze(SyntaxNodeAnalysisContext context)
    {
    }

    /// <summary>
    /// Get the syntax kind to analyze.
    /// </summary>
    /// <returns>The list of syntax kind.</returns>
    protected virtual SyntaxKind[] GetSyntaxKind() =>
    [
        SyntaxKind.InvocationExpression
    ];

    private SyntaxKind[] GetKind() => GetSyntaxKind();
}