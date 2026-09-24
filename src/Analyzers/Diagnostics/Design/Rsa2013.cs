using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA2013"/>.
/// </summary>
/// <remarks>
/// The limit comes from the <c>max_line_length</c> editorconfig key, the same key Rider and
/// ReSharper read, so the margin the editor draws and the margin the build enforces are the same
/// number. Where the key is absent, or set to <c>off</c>, the rule reports nothing rather than
/// inventing a limit.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa2013 : Rsa2000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA2013];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var walk = DocumentWalk.Of(context);
        if (walk.Limit is not { } limit)
        {
            return;
        }

        var tree = context.Node.SyntaxTree;

        foreach (var overlong in walk.OverlongLines)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(RSA2013, Location.Create(tree, overlong.Span), overlong.Length, limit));
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.CompilationUnit];
}
