using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Rocket.Surgery.Airframe.Analyzers;

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
        if (root == null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var token = root.FindToken(diagnosticSpan.Start);
        var lambda = token.Parent?.AncestorsAndSelf().OfType<LambdaExpressionSyntax>().FirstOrDefault();
        if (lambda == null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => MakeLambdaStaticAsync(context.Document, lambda, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> MakeLambdaStaticAsync(Document document, LambdaExpressionSyntax lambda, CancellationToken cancellationToken)
    {
        var staticKeyword = Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(Space);

        var newModifiers = lambda.Modifiers.Add(staticKeyword);
        var newLambda = lambda.WithModifiers(newModifiers)
           .WithAdditionalAnnotations(Formatter.Annotation);

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null)
        {
            return document;
        }

        var newRoot = root.ReplaceNode(lambda, newLambda);
        return document.WithSyntaxRoot(newRoot);
    }

    private const string Title = "Make lambda expression static";
}