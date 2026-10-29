using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Enumerates the types a compilation unit declares at the top level, meaning types that are not
/// nested inside another type. Shared by RSA2008 and RSA2009.
/// </summary>
internal static class TopLevelTypes
{
    /// <summary>
    /// Enumerates the top level types of a compilation unit, in declaration order.
    /// </summary>
    /// <param name="compilationUnit">The compilation unit.</param>
    /// <returns>The top level type declarations.</returns>
    public static IEnumerable<MemberDeclarationSyntax> Of(CompilationUnitSyntax compilationUnit) => In(compilationUnit.Members);

    private static IEnumerable<MemberDeclarationSyntax> In(IEnumerable<MemberDeclarationSyntax> members)
    {
        foreach (var member in members)
        {
            switch (member)
            {
                case BaseNamespaceDeclarationSyntax @namespace:
                    foreach (var nested in In(@namespace.Members))
                    {
                        yield return nested;
                    }

                    break;
                case BaseTypeDeclarationSyntax:
                case DelegateDeclarationSyntax:
                    yield return member;
                    break;
            }
        }
    }
}
