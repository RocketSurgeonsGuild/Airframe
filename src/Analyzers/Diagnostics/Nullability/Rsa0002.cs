using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0002"/>.
/// </summary>
/// <remarks>
/// Reports on a public or protected method or property whose declared return type is a nullable
/// collection - an array, or a type assignable to the non-generic <c>IEnumerable</c>, excluding
/// <c>string</c>. An override and an explicit interface implementation are excluded, since neither
/// author chose the nullability of the contract they are fulfilling.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0002 : Rsa0000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0002];

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
            BoundaryMembers.IsInheritedContract(symbol))
        {
            return;
        }

        var type = symbol switch
        {
            IMethodSymbol method => method.ReturnType,
            IPropertySymbol property => property.Type,
            var _ => null
        };

        if (type is null || !BoundaryMembers.IsNullableCollectionType(type))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA0002, typeSyntax.GetLocation(), symbol.Name));
    }
}
