using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Design;

/// <summary>
/// Represents a code fix for <see cref="Descriptions.RSA2015"/>.
/// </summary>
/// <remarks>
/// Adds <c>/// &lt;inheritdoc /&gt;</c> above the member, ahead of any attribute list. A member that
/// already carries a documentation comment is left alone, since replacing prose the author wrote
/// is not this fix's call.
/// </remarks>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa2015Fix))]
public class Rsa2015Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [RSA2015.Id];

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

        var member = root
           .FindToken(diagnostic.Location.SourceSpan.Start)
           .Parent?
           .AncestorsAndSelf()
           .OfType<MemberDeclarationSyntax>()
           .FirstOrDefault();

        if (member == null || member.GetLeadingTrivia().Any(trivia => trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia)))
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => AddAsync(context.Document, root, member, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static Task<Document> AddAsync(Document document, SyntaxNode root, MemberDeclarationSyntax member, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var leading = member.GetLeadingTrivia();

        // Reuse the member's own indentation, the whitespace on its last line of leading trivia,
        // so the comment lines up whatever the nesting depth.
        var indent = leading.LastOrDefault(trivia => trivia.IsKind(SyntaxKind.WhitespaceTrivia));
        var newLine = member.GetFirstToken().GetPreviousToken().TrailingTrivia.LastOrDefault(trivia => trivia.IsKind(SyntaxKind.EndOfLineTrivia));
        var endOfLine = newLine.IsKind(SyntaxKind.EndOfLineTrivia) ? newLine : ElasticCarriageReturnLineFeed;

        var comment = ParseLeadingTrivia("/// <inheritdoc />" + endOfLine.ToFullString())
           .Where(trivia => trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
           .ToList();

        var withDoc = leading
           .Where(trivia => !trivia.IsKind(SyntaxKind.WhitespaceTrivia) || trivia != indent)
           .ToList();
        withDoc.AddRange(new[] { indent }.Where(trivia => trivia != default));
        withDoc.AddRange(comment);
        withDoc.AddRange(new[] { indent }.Where(trivia => trivia != default));

        var documented = member.WithLeadingTrivia(TriviaList(withDoc));

        return Task.FromResult(
            document.WithSyntaxRoot(root.ReplaceNode(member, documented.WithAdditionalAnnotations(Formatter.Annotation))));
    }

    private const string Title = "Add inheritdoc";
}
