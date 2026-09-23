using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Finds preprocessor directives whose meaning depends on where they sit among the members of a
/// type, so a reorder cannot move a member past them without changing what the code means.
/// </summary>
/// <remarks>
/// Two shapes qualify. A directive pair that opens inside one member and closes inside another, or
/// that closes on the type's own brace, wraps whole member declarations; whichever member moved
/// past it would leave or join the conditional block. A warning pragma written between two members
/// starts suppressing from that point, so moving the member that carries it moves where the
/// suppression begins. A pair contained entirely within one member, or a pragma inside a member
/// body, travels with that member and means the same thing wherever it lands.
///
/// RSA2012 reports the conditional case. The member reorder declines across either.
/// </remarks>
internal static class DirectiveSpan
{
    /// <summary>
    /// Determines whether any directive depends on its position among the members.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>A value indicating whether a reorder would be unsafe.</returns>
    public static bool SpansMembers(TypeDeclarationSyntax type) => Crossing(type).Any();

    /// <summary>
    /// Enumerates the opening directive of every construct that depends on its position among the
    /// members, so one straddling block produces one result rather than one per directive.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>The offending directives.</returns>
    public static IEnumerable<DirectiveTriviaSyntax> Crossing(TypeDeclarationSyntax type)
    {
        if (!type.ContainsDirectives)
        {
            yield break;
        }

        var owner = new Dictionary<DirectiveTriviaSyntax, int>();

        for (var index = 0; index < type.Members.Count; index++)
        {
            foreach (var directive in DirectivesIn(type.Members[index]))
            {
                owner[directive] = index;
            }
        }

        foreach (var directive in DirectivesIn(type))
        {
            var related = directive.GetRelatedDirectives();

            // Report at the opening directive only.
            if (related.Count > 0 && related[0] != directive)
            {
                continue;
            }

            if (Crosses(directive, related, owner) || IsPositionalPragma(directive, type, owner))
            {
                yield return directive;
            }
        }
    }

    private static bool Crosses(
        DirectiveTriviaSyntax directive,
        IReadOnlyList<DirectiveTriviaSyntax> related,
        Dictionary<DirectiveTriviaSyntax, int> owner)
    {
        // A directive owned by no member sits on the type's own braces, so every member is on one
        // side of it or the other.
        if (!owner.TryGetValue(directive, out var index))
        {
            return true;
        }

        return related.Any(partner => !owner.TryGetValue(partner, out var partnerIndex) || partnerIndex != index);
    }

    /// <summary>
    /// Determines whether a warning pragma sits in the gap between two members.
    /// </summary>
    /// <remarks>
    /// Roslyn does not relate a warning disable to its restore, so a straddling pragma pair is
    /// invisible to <c>GetRelatedDirectives</c> and has to be recognised by position instead. A
    /// member's span excludes its leading trivia, so a pragma the span does not contain is one
    /// written between that member and the one before it.
    /// </remarks>
    /// <param name="directive">The directive.</param>
    /// <param name="type">The type being inspected.</param>
    /// <param name="owner">The member each directive belongs to.</param>
    /// <returns>A value indicating whether the pragma depends on its position.</returns>
    private static bool IsPositionalPragma(
        DirectiveTriviaSyntax directive,
        TypeDeclarationSyntax type,
        Dictionary<DirectiveTriviaSyntax, int> owner)
    {
        if (directive is not PragmaWarningDirectiveTriviaSyntax)
        {
            return false;
        }

        // An unowned pragma is already caught as a crossing directive.
        return owner.TryGetValue(directive, out var index)
         && !type.Members[index].Span.Contains(directive.SpanStart);
    }

    private static IEnumerable<DirectiveTriviaSyntax> DirectivesIn(SyntaxNode node) =>
        node.DescendantNodes(descendIntoTrivia: true).OfType<DirectiveTriviaSyntax>();
}
