using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0009"/>.
/// </summary>
/// <remarks>
/// Reports on an abstract, virtual, or interface method, property, or indexer whose declared
/// return type is a nullable reference type. A collection-shaped or task-shaped return type is
/// excluded, since RSA0002 and RSA0003 already report those at <see cref="DiagnosticSeverity.Warning"/>;
/// this rule covers everything else. An override and an explicit interface implementation are
/// excluded via <see cref="BoundaryMembers.IsInheritedContract"/> - a re-abstracting
/// <c>abstract override</c> member is both an override and, per
/// <see cref="BoundaryMembers.IsAbstractionOrigin"/>, an abstraction origin at the same time, and
/// without this check it reported on both the member that originated the nullable return and the
/// one that merely restates it. Ships disabled by default - see <see cref="Descriptions.RSA0009"/>.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0009 : BoundaryReturnTypeRule
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0009];

    /// <inheritdoc/>
    protected override DiagnosticDescriptor Descriptor => RSA0009;

    /// <inheritdoc/>
    protected override bool IsEligible(ISymbol symbol) => BoundaryMembers.IsAbstractionOrigin(symbol);

    /// <inheritdoc/>
    protected override bool Matches(ITypeSymbol type, out object?[] extraMessageArgs)
    {
        extraMessageArgs = [];

        return type.NullableAnnotation == NullableAnnotation.Annotated &&
            type.IsReferenceType &&
            !BoundaryMembers.IsNullableCollectionType(type) &&
            !BoundaryMembers.IsNullableTaskType(type, out _);
    }
}
