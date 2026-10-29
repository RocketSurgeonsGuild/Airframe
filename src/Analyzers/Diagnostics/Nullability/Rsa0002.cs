using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0002"/>.
/// </summary>
/// <remarks>
/// Reports on a public or protected method, property, or indexer whose declared return type is a
/// nullable collection - an array, or a type assignable to the non-generic <c>IEnumerable</c>,
/// excluding <c>string</c>. An override and an explicit interface implementation are excluded,
/// since neither author chose the nullability of the contract they are fulfilling.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0002 : BoundaryReturnTypeRule
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0002];

    /// <inheritdoc/>
    protected override DiagnosticDescriptor Descriptor => RSA0002;

    /// <inheritdoc/>
    protected override bool Matches(ITypeSymbol type, out object?[] extraMessageArgs)
    {
        extraMessageArgs = [];

        return BoundaryMembers.IsNullableCollectionType(type);
    }
}
