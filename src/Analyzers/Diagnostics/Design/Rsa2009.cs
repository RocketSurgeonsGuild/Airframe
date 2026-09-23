using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA2009"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa2009 : Rsa2000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA2009];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not CompilationUnitSyntax compilationUnit)
        {
            return;
        }

        var path = context.Node.SyntaxTree.FilePath;

        // In memory compilations carry no path; there is nothing to match against.
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var first = TopLevelTypes.Of(compilationUnit).FirstOrDefault();
        if (first == null)
        {
            return;
        }

        var fileName = Path.GetFileNameWithoutExtension(path);
        var expected = MemberRank.NameOf(first);

        if (Normalize(fileName).Equals(expected, StringComparison.Ordinal))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(RSA2009, MemberRank.LocationOf(first), fileName, expected));
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.CompilationUnit];

    /// <summary>
    /// Reduces a file name to the type name it claims.
    /// </summary>
    /// <remarks>
    /// Two conventions are honoured. A generic type may spell its arity, so <c>Thing{T}.cs</c> and
    /// <c>Thing`1.cs</c> both name <c>Thing</c>. A partial type may split across files with a
    /// suffix, so <c>Thing.Statics.cs</c> and <c>Thing+Statics.cs</c> also name <c>Thing</c>.
    /// </remarks>
    /// <param name="fileName">The file name without its extension.</param>
    /// <returns>The bare type name.</returns>
    private static string Normalize(string fileName)
    {
        var end = fileName.IndexOfAny(Separators);
        return end >= 0 ? fileName.Substring(0, end) : fileName;
    }

    private static readonly char[] Separators = ['{', '`', '+', '.'];
}
