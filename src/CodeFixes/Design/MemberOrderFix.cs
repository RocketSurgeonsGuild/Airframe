using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

namespace Rocket.Surgery.Airframe.CodeFixes.Design;

/// <summary>
/// Represents a code fix for the member ordering diagnostics <see cref="Descriptions.RSA2001"/>
/// through <see cref="Descriptions.RSA2007"/>.
/// </summary>
/// <remarks>
/// One fix serves all seven diagnostics because there is one sort. It is named for what it does
/// rather than for a rule number, since <c>Rsa2001Fix</c> would suggest it only resolves RSA2001.
///
/// The sort applies the whole layout. A consumer who leaves RSA2003 on but switches RSA2004 off
/// still gets members ordered by access when they invoke this fix; the ordering rules are meant to
/// be tuned by severity individually and enabled as a group.
/// </remarks>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberOrderFix))]
public class MemberOrderFix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
    [
        RSA2001.Id,
        RSA2002.Id,
        RSA2003.Id,
        RSA2004.Id,
        RSA2005.Id,
        RSA2006.Id,
        RSA2007.Id
    ];

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];
        var node = root.FindNode(diagnostic.Location.SourceSpan);

        // Ancestors, not AncestorsAndSelf, so a flagged nested type resolves to the type that
        // contains it rather than to itself.
        var type = node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
        if (type == null)
        {
            return;
        }

        // Reordering across a conditional block would move a member into or out of it and quietly
        // change which symbols compile it. Report the diagnostic, but leave the edit to a human.
        if (DirectivesSpanMembers(type))
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => ReorderAsync(context.Document, root, type, c),
                equivalenceKey: Title),
            diagnostic);
    }

    /// <summary>
    /// Determines whether a preprocessor directive crosses the boundary between the members of a
    /// type.
    /// </summary>
    /// <remarks>
    /// A directive pair that opens inside one member and closes inside another, or that closes on
    /// the type's own brace, cannot survive a reorder: whichever member moves past it leaves or
    /// joins the conditional block, so the code still compiles but no longer compiles under the
    /// same symbols. A pair contained entirely within one member travels with that member and is
    /// safe. A directive with no partner, such as a lone pragma, is treated as safe because it
    /// moves with the member that carries it.
    /// </remarks>
    /// <param name="type">The type whose members would be reordered.</param>
    /// <returns>A value indicating whether a directive spans members.</returns>
    private static bool DirectivesSpanMembers(TypeDeclarationSyntax type)
    {
        if (!type.ContainsDirectives)
        {
            return false;
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
            // A directive that belongs to no member sits on the type's own braces, so any member
            // that moves past it crosses it.
            if (!owner.TryGetValue(directive, out var index))
            {
                return true;
            }

            foreach (var related in directive.GetRelatedDirectives())
            {
                if (!owner.TryGetValue(related, out var relatedIndex) || relatedIndex != index)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static IEnumerable<DirectiveTriviaSyntax> DirectivesIn(SyntaxNode node) =>
        node.DescendantNodes(descendIntoTrivia: true).OfType<DirectiveTriviaSyntax>();

    private static Task<Document> ReorderAsync(Document document, SyntaxNode root, TypeDeclarationSyntax type, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Stable sort: members of equal rank keep their original relative order, so two public
        // methods are never reshuffled.
        var sorted = type
           .Members
           .Select((member, index) => (member, index, rank: MemberRank.Of(member)))
           .OrderBy(x => x.rank)
           .ThenBy(x => x.index)
           .Select(x => x.member);

        // XML docs and comments are leading trivia of the member, and attribute lists are child
        // nodes of it. Both are part of the member node, so both travel with it.
        var reordered = type
           .WithMembers(Separate(sorted, NewLineOf(type)))
           .WithAdditionalAnnotations(Formatter.Annotation);

        return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(type, reordered)));
    }

    /// <summary>
    /// Rebuilds the blank line between members. A member's blank line is part of its leading
    /// trivia, so without this the line that used to separate the first member from the brace
    /// travels with that member and the members it was moved past end up glued together.
    /// </summary>
    private static SyntaxList<MemberDeclarationSyntax> Separate(IEnumerable<MemberDeclarationSyntax> members, SyntaxTrivia newLine)
    {
        var separated = new List<MemberDeclarationSyntax>();
        var first = true;

        foreach (var member in members)
        {
            var leading = member.GetLeadingTrivia();
            var index = 0;

            while (index < leading.Count)
            {
                if (leading[index].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    index++;
                    continue;
                }

                if (leading[index].IsKind(SyntaxKind.WhitespaceTrivia) &&
                    index + 1 < leading.Count &&
                    leading[index + 1].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    index += 2;
                    continue;
                }

                break;
            }

            var trimmed = SyntaxFactory.TriviaList(leading.Skip(index));

            separated.Add(member.WithLeadingTrivia(first ? trimmed : trimmed.Insert(0, newLine)));
            first = false;
        }

        return SyntaxFactory.List(separated);
    }

    /// <summary>
    /// Gets the line ending the type already uses, so a reorder does not introduce a second
    /// convention into the file.
    /// </summary>
    private static SyntaxTrivia NewLineOf(TypeDeclarationSyntax type)
    {
        foreach (var trivia in type.DescendantTrivia())
        {
            if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                return trivia;
            }
        }

        return SyntaxFactory.CarriageReturnLineFeed;
    }

    private const string Title = "Reorder members to match the layout";
}
