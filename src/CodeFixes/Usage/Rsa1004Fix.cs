using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Usage;

/// <summary>
/// Represents a code fix for <see cref="Descriptions.RSA1004"/>.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa1004Fix))]
public class Rsa1004Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the type declaration identified by the diagnostic.
        var ancestorsAndSelf = (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf() ?? []).ToList();
        var declaration = ancestorsAndSelf.OfType<SimpleLambdaExpressionSyntax>().First();

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => Fix(context, declaration, c),
                equivalenceKey: RSA1003.Id + RSA1003.Title),
            diagnostic);
    }

    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [RSA1004.Id];

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    private static async Task<Document> Fix(
        CodeFixContext context,
        SimpleLambdaExpressionSyntax declaration,
        CancellationToken cancellationToken)
    {
        var rootAsync = await context.Document.GetSyntaxRootAsync(cancellationToken);
        if (rootAsync == null)
        {
            return context.Document;
        }

        var simpleMemberAccessExpression = rootAsync.FindNode(context.Span);

        var memberAccessExpression =
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(declaration.Parameter.Identifier),
                IdentifierName(declaration.Body.GetLastToken()));

        var changed = rootAsync.ReplaceNode(simpleMemberAccessExpression, memberAccessExpression);
        return context.Document.WithSyntaxRoot(changed);
    }

    private const string Title = "Add required member access prefix on expression lambda";
}