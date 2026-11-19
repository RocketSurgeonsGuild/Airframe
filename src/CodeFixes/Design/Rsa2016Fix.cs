using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

namespace Rocket.Surgery.Airframe.CodeFixes.Design;

/// <summary>Removes a redundant type alias while retaining comments and surrounding trivia.</summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa2016Fix))]
public sealed class Rsa2016Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds => [Descriptions.RSA2016.Id];

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null || model == null)
        {
            return;
        }

        foreach (var directive in RedundantAliases.Find(model, context.CancellationToken))
        {
            var diagnostic = context.Diagnostics.FirstOrDefault(item => item.Location.SourceSpan == directive.Span);
            if (diagnostic == null)
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    Title,
                    cancellationToken => RemoveAsync(context.Document, root, directive, cancellationToken),
                    equivalenceKey: Title),
                diagnostic);
        }
    }

    private static Task<Document> RemoveAsync(Document document, SyntaxNode root, UsingDirectiveSyntax directive, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(document.WithSyntaxRoot(root.RemoveNode(directive, SyntaxRemoveOptions.KeepExteriorTrivia)!));
    }

    private const string Title = "Remove redundant type alias";
}
