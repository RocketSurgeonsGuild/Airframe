using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0001"/>.
/// </summary>
/// <remarks>
/// Reports on a public or protected auto-property of non-nullable reference type whose setter is a
/// plain <c>set</c> - not <c>init</c>, which already restricts assignment to construction, and not
/// a property with a hand-written accessor body, since a manual setter may already guard against
/// null itself. The setter's own accessibility must be at least as visible as the property, so a
/// narrowed <c>private set</c> or <c>internal set</c> is excluded - external code was never able
/// to reach it in the first place. An override and an explicit interface implementation are
/// excluded, since neither author chose that member's mutability. A property on a type that implements
/// <see cref="System.ComponentModel.INotifyPropertyChanged"/>, or one carrying a <c>[Reactive]</c>
/// attribute, is excluded as well - a bindable view-model property is expected to be externally
/// mutable by design, and without this exclusion the rule would indict this framework's own
/// ReactiveObject-derived view models on first run.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0001 : Rsa0000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0001];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not PropertyDeclarationSyntax property)
        {
            return;
        }

        if (FindPlainSetAccessor(property) is not { } setAccessor)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(property, context.CancellationToken) is not IPropertySymbol symbol ||
            !BoundaryMembers.IsPubliclyVisible(symbol) ||
            symbol.SetMethod is not { } setMethod ||
            !BoundaryMembers.IsPubliclyVisible(setMethod) ||
            BoundaryMembers.IsInheritedContract(symbol) ||
            BoundaryMembers.IsBindableViewModelMember(symbol))
        {
            return;
        }

        var type = symbol.Type;
        if (type.NullableAnnotation == NullableAnnotation.Annotated || !type.IsReferenceType)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA0001, setAccessor.GetLocation(), symbol.Name));
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.PropertyDeclaration];

    /// <summary>
    /// Finds the property's <c>set</c> accessor when the property is an auto-property - every
    /// accessor it declares has no body and no expression body, which is the only shape a plain
    /// <c>set</c> can share a declaration with an auto-getter (C# does not allow mixing an
    /// auto-accessor with a hand-written one on the same property).
    /// </summary>
    private static AccessorDeclarationSyntax? FindPlainSetAccessor(PropertyDeclarationSyntax property)
    {
        if (property.AccessorList is not { } accessorList)
        {
            return null;
        }

        AccessorDeclarationSyntax? setAccessor = null;

        foreach (var accessor in accessorList.Accessors)
        {
            if (accessor.Body is not null || accessor.ExpressionBody is not null)
            {
                return null;
            }

            if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
            {
                setAccessor = accessor;
            }
        }

        return setAccessor;
    }
}
