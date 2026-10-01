using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents an RSA 0000 level analyzer.
/// </summary>
/// <remarks>
/// Generated code analysis is switched off. The rules in this band are declaration-site
/// nullability contracts, which are only meaningful for a member an author wrote; a generator
/// owns the shape of its own output, so reporting RSA0XXX on a <c>.g.cs</c> file would produce
/// warnings nobody can act on. The default syntax kinds are method and property declarations,
/// because every rule in this band inspects a member's declared return or property type.
/// </remarks>
public abstract class Rsa0000 : DiagnosticAnalyzer
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
    protected abstract void Analyze(SyntaxNodeAnalysisContext context);

    /// <summary>
    /// Get the syntax kind to analyze.
    /// </summary>
    /// <returns>The list of syntax kind.</returns>
    protected virtual SyntaxKind[] GetSyntaxKind() =>
    [
        SyntaxKind.MethodDeclaration,
        SyntaxKind.PropertyDeclaration
    ];

    private SyntaxKind[] GetKind() => GetSyntaxKind();
}
