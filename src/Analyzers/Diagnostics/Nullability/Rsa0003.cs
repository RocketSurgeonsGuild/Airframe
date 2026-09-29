using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0003"/>.
/// </summary>
/// <remarks>
/// Reports on a public or protected method, property, or indexer whose declared return type is a
/// nullable <c>Task</c>, <c>Task&lt;T&gt;</c>, <c>ValueTask</c>, or <c>ValueTask&lt;T&gt;</c>. A
/// nullable type argument, e.g. <c>Task&lt;T?&gt;</c>, is a nullable result rather than a nullable
/// task and is deliberately not reported. An override and an explicit interface implementation are
/// excluded, since neither author chose the nullability of the contract they are fulfilling.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0003 : Rsa0000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0003];

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
            case IndexerDeclarationSyntax indexer:
                Analyze(context, indexer, indexer.Type);
                break;
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() =>
    [
        SyntaxKind.MethodDeclaration,
        SyntaxKind.PropertyDeclaration,
        SyntaxKind.IndexerDeclaration
    ];

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

        if (type is null || !BoundaryMembers.IsNullableTaskType(type, out var taskName))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA0003, typeSyntax.GetLocation(), symbol.Name, taskName));
    }
}
