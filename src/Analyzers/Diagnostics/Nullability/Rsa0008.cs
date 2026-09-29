using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0008"/>.
/// </summary>
/// <remarks>
/// Walks every <c>#nullable</c> directive in the file and reports on a <c>disable</c> or
/// <c>restore</c> setting, unless its target is specifically <c>warnings</c> - a
/// <c>#nullable disable warnings</c> or <c>#nullable restore warnings</c> leaves the annotation
/// context itself untouched, which is the half every other RSA0XXX rule in this analyzer depends
/// on. An unqualified <c>#nullable disable</c>/<c>restore</c>, and one explicitly qualified
/// <c>annotations</c>, are both reported.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0008 : Rsa0000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0008];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not CompilationUnitSyntax compilationUnit)
        {
            return;
        }

        foreach (var directive in compilationUnit
                   .DescendantTrivia(descendIntoTrivia: true)
                   .Where(trivia => trivia.IsKind(SyntaxKind.NullableDirectiveTrivia))
                   .Select(trivia => trivia.GetStructure())
                   .OfType<NullableDirectiveTriviaSyntax>())
        {
            if (directive.SettingToken.IsKind(SyntaxKind.EnableKeyword) ||
                directive.TargetToken.IsKind(SyntaxKind.WarningsKeyword))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(RSA0008, directive.GetLocation(), directive.SettingToken.Text));
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.CompilationUnit];
}
