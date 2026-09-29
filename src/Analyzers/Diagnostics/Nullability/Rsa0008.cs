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
/// <c>annotations</c>, are both reported. Matching is a positive check against
/// <c>DisableKeyword</c>/<c>RestoreKeyword</c>, not a negative check against
/// <c>EnableKeyword</c> - a directive typed only as far as <c>#nullable</c>, with no setting
/// keyword yet, is deliberately not reported, since a negative check would treat "not yet enable"
/// as "disable" while the author is still mid-keystroke. Getting this right needs both an
/// <see cref="SyntaxKind"/> check and an <see cref="SyntaxToken.IsMissing"/> check: confirmed by
/// inspecting the parsed tree directly, Roslyn's error recovery for that incomplete directive
/// synthesizes a zero-width <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.SettingToken"/>
/// whose <em>kind</em> is <c>DisableKeyword</c> - not <c>None</c> - with <c>IsMissing</c> set to
/// <see langword="true"/>. A kind-only check reads that recovery token as a real
/// <c>#nullable disable</c>.
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
            var disablesOrRestores = !directive.SettingToken.IsMissing &&
                (directive.SettingToken.IsKind(SyntaxKind.DisableKeyword) || directive.SettingToken.IsKind(SyntaxKind.RestoreKeyword));

            if (!disablesOrRestores || directive.TargetToken.IsKind(SyntaxKind.WarningsKeyword))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(RSA0008, directive.GetLocation(), directive.SettingToken.Text));
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.CompilationUnit];
}
