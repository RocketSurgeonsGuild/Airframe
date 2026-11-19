using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;

namespace Rocket.Surgery.Airframe.CodeFixes.Performance;

/// <summary>
/// Represents a code fix for <see cref="Descriptions.RSA3002"/>.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa3002Fix))]
public class Rsa3002Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [RSA3002.Id];

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null || model == null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var token = root.FindToken(diagnosticSpan.Start);
        var function = token.Parent?.AncestorsAndSelf().OfType<AnonymousFunctionExpressionSyntax>().FirstOrDefault();
        if (function == null || function.Span != diagnosticSpan
            || !StaticAnonymousFunctions.CanMakeStatic(model, function, context.CancellationToken))
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => MakeFunctionStaticAsync(context.Document, function, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> MakeFunctionStaticAsync(Document document, AnonymousFunctionExpressionSyntax function, CancellationToken cancellationToken)
    {
        var first = function.GetFirstToken();
        var leading = first.LeadingTrivia;
        var stripped = function.ReplaceToken(first, first.WithLeadingTrivia(TriviaList()));
        var staticKeyword = Token(leading, SyntaxKind.StaticKeyword, TriviaList(Space));

        var newModifiers = stripped.Modifiers.Insert(0, staticKeyword);
        var newFunction = stripped.WithModifiers(newModifiers)
           .WithAdditionalAnnotations(Formatter.Annotation);

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null)
        {
            return document;
        }

        var newRoot = root.ReplaceNode(function, newFunction);
        return document.WithSyntaxRoot(newRoot);
    }

    private const string Title = "Make lambda expression static";
}
