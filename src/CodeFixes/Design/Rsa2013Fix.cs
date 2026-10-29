using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Formatting;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Design;

/// <summary>
/// Represents a code fix for <see cref="Descriptions.RSA2013"/>.
/// </summary>
/// <remarks>
/// Breaks the line at the outermost construct that can carry a break, which is what Rider's chop
/// line does. Nothing is offered for a line whose length is a string literal or a comment, because
/// there is no break to insert that would not change the text.
/// </remarks>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa2013Fix))]
public class Rsa2013Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [RSA2013.Id];

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

        var text = await context.Document.GetTextAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics[0];
        var line = text.Lines.GetLineFromPosition(diagnostic.Location.SourceSpan.Start);

        // Ask for the rewrite up front. A line carrying its length in a string or a comment has
        // nothing to break, and offering a fix that does nothing is worse than offering none.
        if (LineChopper.Chop(root, text, line) == null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => ChopAsync(context.Document, line, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> ChopAsync(Document document, Microsoft.CodeAnalysis.Text.TextLine line, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

        if (root == null)
        {
            return document;
        }

        var chopped = LineChopper.Chop(root, text, line);

        return chopped == null
            ? document
            : document.WithSyntaxRoot(chopped.WithAdditionalAnnotations(Formatter.Annotation));
    }

    private const string Title = "Chop line";
}
