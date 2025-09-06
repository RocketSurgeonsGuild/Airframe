using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Usage;

/// <summary>
/// Represents a code fix for <see cref="Descriptions.RSA1001"/>.
/// </summary>
public class Rsa1001Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the type declaration identified by the diagnostic.
        var ancestorsAndSelf = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf() ?? [];
        var invocation = ancestorsAndSelf.OfType<InvocationExpressionSyntax>().First();

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => Fixup(context.Document, invocation, c),
                equivalenceKey: Descriptions.RSA1001.Id + Descriptions.RSA1001.Title),
            diagnostic);
    }

    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [Descriptions.RSA1001.Id];

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    private static async Task<Document> Fix(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
    {
        var rootAsync = await document.GetSyntaxRootAsync(cancellationToken);
        if (rootAsync == null)
        {
            return document;
        }

        var argumentSyntax = invocation.ArgumentList.Arguments.First();
        var commandName = argumentSyntax.Expression.GetText();
        var arguments = ArgumentList(
            SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                    Argument(ThisExpression()),
                    Token(
                        TriviaList(),
                        SyntaxKind.CommaToken,
                        TriviaList(Space)),
                    Argument(
                        SimpleLambdaExpression(
                                Parameter(
                                    Identifier(
                                        TriviaList(),
                                        "x",
                                        TriviaList(Space))))
                           .WithArrowToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.EqualsGreaterThanToken,
                                    TriviaList(Space)))
                           .WithExpressionBody(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("x"),
                                    IdentifierName(commandName.ToString()))))
                }));

        var changed = rootAsync.ReplaceNode(invocation.ArgumentList, arguments);

        return document.WithSyntaxRoot(changed);
    }

    private Task<Document> Fixup(
        Document document,
        InvocationExpressionSyntax invocation,
        CancellationToken cancellationToken) => Fix(document, invocation, cancellationToken);

    private const string Title = "Use lambda expression syntax";
}