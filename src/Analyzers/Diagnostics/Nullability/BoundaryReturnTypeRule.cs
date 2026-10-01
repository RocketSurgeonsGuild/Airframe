using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Template Method base for an RSA0XXX rule that reports on a public or protected method,
/// property, or indexer whose declared return (or property/indexer) type matches some
/// nullability-shape predicate. Owns the member-kind switch, visibility/inherited-contract
/// gating, and the symbol-to-declared-type extraction shared by RSA0002, RSA0003, RSA0006, and
/// RSA0009 -- each of those differs only in which types it registers for (<see cref="GetSyntaxKind"/>),
/// whether a member needs an extra eligibility check beyond visibility and inheritance (<see
/// cref="IsEligible"/>), and what counts as a violation (<see cref="Matches"/>).
/// </summary>
public abstract class BoundaryReturnTypeRule : Rsa0000
{
    /// <inheritdoc/>
    protected sealed override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var (member, typeSyntax) = context.Node switch
        {
            MethodDeclarationSyntax method => ((MemberDeclarationSyntax)method, method.ReturnType),
            PropertyDeclarationSyntax property => (property, property.Type),
            IndexerDeclarationSyntax indexer => (indexer, indexer.Type),
            var _ => (null, null),
        };

        if (member is null || typeSyntax is null)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(member, context.CancellationToken) is not { } symbol ||
            !BoundaryMembers.IsPubliclyVisible(symbol) ||
            BoundaryMembers.IsInheritedContract(symbol) ||
            !IsEligible(symbol))
        {
            return;
        }

        var type = symbol switch
        {
            IMethodSymbol method => method.ReturnType,
            IPropertySymbol property => property.Type,
            var _ => null,
        };

        if (type is null || !Matches(type, out var extraMessageArgs))
        {
            return;
        }

        object?[] messageArgs = [symbol.Name, .. extraMessageArgs];
        context.ReportDiagnostic(Diagnostic.Create(Descriptor, typeSyntax.GetLocation(), messageArgs));
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() =>
    [
        SyntaxKind.MethodDeclaration,
        SyntaxKind.PropertyDeclaration,
        SyntaxKind.IndexerDeclaration
    ];

    /// <summary>
    /// The descriptor this rule reports when <see cref="Matches"/> returns <see langword="true"/>.
    /// </summary>
    protected abstract DiagnosticDescriptor Descriptor { get; }

    /// <summary>
    /// An additional eligibility check beyond visibility and <see cref="BoundaryMembers.IsInheritedContract"/>.
    /// Default: every publicly visible, non-inherited member is eligible.
    /// </summary>
    /// <param name="symbol">The declared member symbol.</param>
    /// <returns><see langword="true"/> when the member should be checked by <see cref="Matches"/>.</returns>
    protected virtual bool IsEligible(ISymbol symbol) => true;

    /// <summary>
    /// Determines whether <paramref name="type"/> is a violation of this rule.
    /// </summary>
    /// <param name="type">The member's declared return (or property/indexer) type.</param>
    /// <param name="extraMessageArgs">Arguments for <see cref="Descriptor"/>'s message format beyond the
    /// member's own name, which the base class always supplies as the first argument.</param>
    /// <returns><see langword="true"/> when <paramref name="type"/> violates this rule.</returns>
    protected abstract bool Matches(ITypeSymbol type, out object?[] extraMessageArgs);
}
