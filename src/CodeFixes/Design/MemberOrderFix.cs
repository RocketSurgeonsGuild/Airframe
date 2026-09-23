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

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => ReorderAsync(context.Document, root, type, c),
                equivalenceKey: Title),
            diagnostic);
    }

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

        // Leading trivia, meaning XML docs, comments and attributes, is part of each member node,
        // so it travels with the member.
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
