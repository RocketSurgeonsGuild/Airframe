using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;

/// <summary>
/// Represents an RSA 1000 level analyzer.
/// </summary>
/// <remarks>
/// Generated code analysis is switched off. The rules in this band catch Rx and MVVM misuse,
/// which is only meaningful in code an author wrote; a generator owns the shape of its own
/// output, so reporting RSA1XXX on a <c>.g.cs</c> file would produce warnings nobody can act on.
/// </remarks>
public abstract class Rsa1000 : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public sealed override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

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