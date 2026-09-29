using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0009"/>.
/// </summary>
/// <remarks>
/// Reports on an abstract, virtual, or interface method or property whose declared return type is
/// a nullable reference type. A collection-shaped or task-shaped return type is excluded, since
/// RSA0002 and RSA0003 already report those at <see cref="DiagnosticSeverity.Warning"/>; this rule
/// covers everything else. Ships disabled by default - see <see cref="Descriptions.RSA0009"/>.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0009 : Rsa0000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0009];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        switch (context.Node)
        {
            case MethodDeclarationSyntax method:
                Analyze(context, method, method.ReturnType);
                break;
            case PropertyDeclarationSyntax property:
                Analyze(context, property, property.Type);
                break;
        }
    }

    private static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax member, TypeSyntax typeSyntax)
    {
        if (context.SemanticModel.GetDeclaredSymbol(member) is not { } symbol ||
            !BoundaryMembers.IsPubliclyVisible(symbol) ||
            !BoundaryMembers.IsAbstractionOrigin(symbol))
        {
            return;
        }

        var type = symbol switch
        {
            IMethodSymbol method => method.ReturnType,
            IPropertySymbol property => property.Type,
            var _ => null
        };

        if (type is null ||
            type.NullableAnnotation != NullableAnnotation.Annotated ||
            !type.IsReferenceType ||
            BoundaryMembers.IsNullableCollectionType(type) ||
            BoundaryMembers.IsNullableTaskType(type, out _))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA0009, typeSyntax.GetLocation(), symbol.Name));
    }
}
