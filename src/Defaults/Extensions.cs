using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Airframe.Defaults.Property;

namespace Rocket.Surgery.Airframe.Defaults;

internal static class Extensions
{
    public static SyntaxList<MemberDeclarationSyntax> GeneratePartialClassWithProperty(
        this INamedTypeSymbol namedTypeSymbol,
        IdentifierNameSyntax className,
        AttributeData attribute,
        ArgumentListSyntax? argumentListSyntax)
    public static bool HasAccessibility(this IMethodSymbol symbol, params Accessibility[] accessibility)
        => accessibility.Contains(symbol.DeclaredAccessibility);

    public static bool HasPublicAccess(this ConstructorDeclarationSyntax syntax) => syntax.Modifiers.Any(token => token.IsKind(SyntaxKind.PublicKeyword));

    public static bool IsDefaultsAttribute(this AttributeSyntax syntax) =>
        ((IdentifierNameSyntax)syntax.Name).Identifier.ValueText is nameof(DefaultsAttribute) or "Defaults";
}