using System;
using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
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
        var tree = context.Node.SyntaxTree;

        if (!TryGetLimit(context.Options.AnalyzerConfigOptionsProvider.GetOptions(tree), out var limit))
        {
            return;
        }

        foreach (var line in tree.GetText(context.CancellationToken).Lines)
        {
            var length = line.End - line.Start;
            if (length <= limit)
            {
                continue;
            }

            // Squiggle only the part past the margin, so the reader sees what has to go.
            context.ReportDiagnostic(
                Diagnostic.Create(
                    RSA2013,
                    Location.Create(tree, TextSpan.FromBounds(line.Start + limit, line.End)),
                    length,
                    limit));
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.CompilationUnit];

    private static bool TryGetLimit(AnalyzerConfigOptions options, out int limit)
    {
        limit = 0;

        return options.TryGetValue("max_line_length", out var value)
         && !string.Equals(value, "off", StringComparison.OrdinalIgnoreCase)
         && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out limit)
         && limit > 0;
    }
}
