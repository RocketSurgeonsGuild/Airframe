using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0006"/>.
/// </summary>
/// <remarks>
/// Reports on a public or protected property or indexer declared <c>bool?</c>. An override and an
/// explicit interface implementation are excluded, since neither author chose that member's type -
/// a legitimate three-state UI binding contract (e.g. a tri-state checkbox) or wire-format DTO
/// where absent-versus-false is itself meaningful is not something this analyzer can distinguish
/// from a genuine smell, so it is left to <c>[SuppressMessage]</c> at the call site instead of an
/// exclusion this analyzer cannot verify.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0006 : BoundaryReturnTypeRule
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0006];

    /// <inheritdoc/>
    protected override DiagnosticDescriptor Descriptor => RSA0006;

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() =>
    [
        SyntaxKind.PropertyDeclaration,
        SyntaxKind.IndexerDeclaration
    ];

    /// <inheritdoc/>
    protected override bool Matches(ITypeSymbol type, out object?[] extraMessageArgs)
    {
        extraMessageArgs = [];

        return BoundaryMembers.IsNullableBoolean(type);
    }
}
