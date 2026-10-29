using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
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
public class Rsa0003 : BoundaryReturnTypeRule
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0003];

    /// <inheritdoc/>
    protected override DiagnosticDescriptor Descriptor => RSA0003;

    /// <inheritdoc/>
    protected override bool Matches(ITypeSymbol type, out object?[] extraMessageArgs)
    {
        if (!BoundaryMembers.IsNullableTaskType(type, out var taskName))
        {
            extraMessageArgs = [];
            return false;
        }

        extraMessageArgs = [taskName];
        return true;
    }
}
