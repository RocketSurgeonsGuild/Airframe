using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Design;

/// <summary>
/// Represents a code fix for <see cref="Descriptions.RSA2010"/>.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa2010Fix))]
public class Rsa2010Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [RSA2010.Id];

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

        if (root.FindTrivia(diagnostic.Location.SourceSpan.Start).GetStructure() is not RegionDirectiveTriviaSyntax region)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => RemoveAsync(context.Document, root, region, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static Task<Document> RemoveAsync(Document document, SyntaxNode root, RegionDirectiveTriviaSyntax region, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // A region and its endregion are one construct; removing only the opener leaves code that
        // does not compile.
        var related = region.GetRelatedDirectives();

        var without = root.RemoveNodes(related, SyntaxRemoveOptions.KeepNoTrivia | SyntaxRemoveOptions.AddElasticMarker);
        if (without == null)
        {
            return Task.FromResult(document);
        }

        return Task.FromResult(document.WithSyntaxRoot(without.WithAdditionalAnnotations(Formatter.Annotation)));
    }

    private const string Title = "Remove the region";
}
