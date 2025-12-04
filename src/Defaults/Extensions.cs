using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Airframe.Defaults.Property;
using Scriban;

namespace Rocket.Surgery.Airframe.Defaults;

internal static class Extensions
{
    /// <summary>
    /// Renders a model given a template.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="template">The template.</param>
    /// <typeparam name="T">The model type.</typeparam>
    /// <returns>A string rendered from the template and model.</returns>
    public static string Render<T>(this T model, string template) => Template.Parse(template).Render(model);

    public static bool HasAccessibility(this IMethodSymbol symbol, params Accessibility[] accessibility)
        => accessibility.Contains(symbol.DeclaredAccessibility);

    public static bool HasPublicAccess(this ConstructorDeclarationSyntax syntax) => syntax.Modifiers.Any(token => token.IsKind(SyntaxKind.PublicKeyword));

    public static bool IsDefaultsAttribute(this AttributeSyntax syntax)
        => ((IdentifierNameSyntax)syntax.Name).Identifier.ValueText is nameof(DefaultsAttribute) or "Defaults";
}