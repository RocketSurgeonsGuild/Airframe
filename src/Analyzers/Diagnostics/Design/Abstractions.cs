using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Identifies interface and abstract type declarations, the abstract or interface members they
/// declare, and the members elsewhere that implement or override them. Shared by RSA2014 and
/// RSA2015.
/// </summary>
internal static class Abstractions
{
    /// <summary>
    /// Determines whether <paramref name="member"/> is part of a public abstraction's contract:
    /// an interface declaration, an abstract type declaration, or a member an interface or an
    /// abstract type declares without a body. RSA2014.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns>A value indicating whether the member is part of an abstraction's contract.</returns>
    public static bool IsAbstraction(MemberDeclarationSyntax member) => member switch
    {
        InterfaceDeclarationSyntax => true,
        ClassDeclarationSyntax @class => @class.Modifiers.Any(SyntaxKind.AbstractKeyword),
        RecordDeclarationSyntax record => record.Modifiers.Any(SyntaxKind.AbstractKeyword),
        _ => IsAbstractMember(member)
    };

    /// <summary>
    /// Determines whether <paramref name="node"/> carries a documentation comment with a
    /// non-empty <c>&lt;summary&gt;</c>. RSA2014.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>A value indicating whether the node has a summary.</returns>
    public static bool HasSummary(SyntaxNode node) =>
        DocumentationOf(node) is { } documentation &&
        documentation
           .Content
           .OfType<XmlElementSyntax>()
           .Any(element =>
                string.Equals(element.StartTag.Name.LocalName.Text, "summary", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(element.Content.ToString()));

    /// <summary>
    /// Determines whether <paramref name="node"/> carries a documentation comment containing
    /// <c>&lt;inheritdoc/&gt;</c>. RSA2015.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>A value indicating whether the node has an inheritdoc tag.</returns>
    public static bool HasInheritdoc(SyntaxNode node) =>
        DocumentationOf(node) is { } documentation &&
        documentation.Content.Any(xml => string.Equals(NameOf(xml), "inheritdoc", StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Finds every member in <paramref name="compilationUnit"/> that implements an interface
    /// member, or overrides an abstract member, and is not itself abstract. RSA2015.
    /// </summary>
    /// <param name="compilationUnit">The compilation unit to walk.</param>
    /// <param name="semanticModel">The semantic model for the compilation unit's syntax tree.</param>
    /// <returns>The member declarations requiring <c>&lt;inheritdoc/&gt;</c>.</returns>
    public static IEnumerable<MemberDeclarationSyntax> FindImplementers(CompilationUnitSyntax compilationUnit, SemanticModel semanticModel)
    {
        foreach (var type in compilationUnit.DescendantNodes().OfType<TypeDeclarationSyntax>())
        {
            if (semanticModel.GetDeclaredSymbol(type) is not { } typeSymbol)
            {
                continue;
            }

            var required = RequiredMembersOf(typeSymbol);
            if (required.Count == 0)
            {
                continue;
            }

            foreach (var member in type.Members)
            {
                if (semanticModel.GetDeclaredSymbol(member) is { } symbol && required.Contains(symbol))
                {
                    yield return member;
                }
            }
        }
    }

    private static HashSet<ISymbol> RequiredMembersOf(INamedTypeSymbol typeSymbol)
    {
        var required = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

        foreach (var @interface in typeSymbol.AllInterfaces)
        {
            foreach (var interfaceMember in @interface.GetMembers())
            {
                if (typeSymbol.FindImplementationForInterfaceMember(interfaceMember) is { IsAbstract: false } implementation &&
                    SymbolEqualityComparer.Default.Equals(implementation.ContainingType, typeSymbol))
                {
                    required.Add(implementation);
                }
            }
        }

        foreach (var member in typeSymbol.GetMembers())
        {
            if (member.IsAbstract)
            {
                continue;
            }

            var overridden = member switch
            {
                IMethodSymbol method => (ISymbol?)method.OverriddenMethod,
                IPropertySymbol property => property.OverriddenProperty,
                IEventSymbol @event => @event.OverriddenEvent,
                _ => null
            };

            if (overridden is { IsAbstract: true })
            {
                required.Add(member);
            }
        }

        return required;
    }

    private static bool IsAbstractMember(MemberDeclarationSyntax member)
    {
        if (member.Parent is InterfaceDeclarationSyntax)
        {
            // A default interface member carries its own body, and a static member is not part
            // of the contract an implementer fulfils, so neither is required to document here.
            return member switch
            {
                MethodDeclarationSyntax declaration => declaration.Body == null && declaration.ExpressionBody == null && !declaration.Modifiers.Any(SyntaxKind.StaticKeyword),
                PropertyDeclarationSyntax declaration => !HasAccessorBody(declaration) && !declaration.Modifiers.Any(SyntaxKind.StaticKeyword),
                IndexerDeclarationSyntax declaration => !HasAccessorBody(declaration),
                EventDeclarationSyntax declaration => !declaration.Modifiers.Any(SyntaxKind.StaticKeyword),
                EventFieldDeclarationSyntax declaration => !declaration.Modifiers.Any(SyntaxKind.StaticKeyword),
                _ => false
            };
        }

        return member.Parent is ClassDeclarationSyntax or RecordDeclarationSyntax &&
               member.Modifiers.Any(SyntaxKind.AbstractKeyword);
    }

    private static bool HasAccessorBody(BasePropertyDeclarationSyntax declaration) =>
        declaration.AccessorList != null &&
        declaration.AccessorList.Accessors.Any(accessor => accessor.Body != null || accessor.ExpressionBody != null);

    private static string? NameOf(XmlNodeSyntax xml) => xml switch
    {
        XmlEmptyElementSyntax empty => empty.Name.LocalName.Text,
        XmlElementSyntax element => element.StartTag.Name.LocalName.Text,
        _ => null
    };

    private static DocumentationCommentTriviaSyntax? DocumentationOf(SyntaxNode node) =>
        node
           .GetLeadingTrivia()
           .Select(trivia => trivia.GetStructure())
           .OfType<DocumentationCommentTriviaSyntax>()
           .FirstOrDefault();
}
