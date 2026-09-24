using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA2012"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa2012 : Rsa2000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA2012];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax type)
        {
            return;
        }

        // Only conditional compilation. A pragma or a region can also straddle members, and the
        // reorder declines across those too, but neither is conditional compilation and neither is
        // fixed by the remedy this rule names. Regions are RSA2010's business.
        foreach (var directive in DirectiveSpan.Crossing(type).OfType<IfDirectiveTriviaSyntax>())
        {
            context.ReportDiagnostic(Diagnostic.Create(RSA2012, directive.GetLocation(), type.Identifier.Text));
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() =>
    [
        SyntaxKind.ClassDeclaration,
        SyntaxKind.StructDeclaration,
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration,

        // Unlike the ordering rules, this one applies to interfaces: hiding half an interface
        // behind a directive is exactly the problem it describes.
        SyntaxKind.InterfaceDeclaration
    ];
}
