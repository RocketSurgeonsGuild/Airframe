using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Represents an RSA 2000 level analyzer.
/// </summary>
/// <remarks>
/// <para>
/// This base deviates from <c>Rsa1000</c> and <c>Rsa3000</c> in two deliberate ways.
/// </para>
/// <para>
/// Generated code analysis is switched off. Layout and structure are authored concerns; a
/// generator owns the shape of its own output, so reporting RSA2XXX on a <c>.g.cs</c> file would
/// produce warnings nobody can act on.
/// </para>
/// <para>
/// The default syntax kinds are type declarations rather than invocation expressions, because
/// every rule in this band is about how a type or a file is laid out. File scoped rules override
/// <see cref="GetSyntaxKind"/> to return <see cref="SyntaxKind.CompilationUnit"/>.
/// </para>
/// </remarks>
public abstract class Rsa2000 : DiagnosticAnalyzer
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
        SyntaxKind.ClassDeclaration,
        SyntaxKind.StructDeclaration,
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration
    ];

    private SyntaxKind[] GetKind() => GetSyntaxKind();
}
