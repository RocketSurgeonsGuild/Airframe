using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA2011"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa2011 : Rsa2000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA2011];

    /// <summary>
    /// Determines whether a member is one the language lets an author give an accessibility to.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns>A value indicating whether accessibility is applicable.</returns>
    internal static bool CanDeclareAccessibility(MemberDeclarationSyntax member)
    {
        switch (member)
        {
            // Namespaces, using directives and enum members have no accessibility at all.
            case BaseNamespaceDeclarationSyntax:
            case EnumMemberDeclarationSyntax:
            case GlobalStatementSyntax:
            case IncompleteMemberSyntax:
            case DestructorDeclarationSyntax:
                return false;

            // A static constructor cannot carry one.
            case ConstructorDeclarationSyntax constructor when constructor.Modifiers.Any(SyntaxKind.StaticKeyword):
                return false;

            // Operators are required to be public static; there is nothing to choose.
            case OperatorDeclarationSyntax:
            case ConversionOperatorDeclarationSyntax:
                return false;
        }

        // Explicit interface implementations must not carry one.
        if (HasExplicitInterfaceSpecifier(member))
        {
            return false;
        }

        // A partial method declaration without a body predates C# 9 and cannot carry one.
        if (member is MethodDeclarationSyntax { Body: null, ExpressionBody: null } method &&
            method.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return false;
        }

        // Interface members are implicitly public.
        return member.Parent is not InterfaceDeclarationSyntax;
    }

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        foreach (var member in DocumentWalk.Of(context).InaccessibleMembers)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(RSA2011, MemberRank.LocationOf(member), MemberRank.NameOf(member)));
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.CompilationUnit];

    private static bool HasExplicitInterfaceSpecifier(MemberDeclarationSyntax member) => member switch
    {
        MethodDeclarationSyntax declaration => declaration.ExplicitInterfaceSpecifier != null,
        PropertyDeclarationSyntax declaration => declaration.ExplicitInterfaceSpecifier != null,
        IndexerDeclarationSyntax declaration => declaration.ExplicitInterfaceSpecifier != null,
        EventDeclarationSyntax declaration => declaration.ExplicitInterfaceSpecifier != null,
        _ => false
    };
}
