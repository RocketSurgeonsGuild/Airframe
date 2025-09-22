using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Performance;

/// <summary>
/// Represents a code fix for <see cref="Descriptions.RSA3001"/>.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa3001Fix))]
public class Rsa3001Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [RSA3001.Id];

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics[0];

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => Fixup(context, c),
                equivalenceKey: RSA3001.Id + RSA3001.Title),
            diagnostic);
    }

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    private async Task<Document> Fixup(CodeFixContext context, CancellationToken cancellationToken)
    {
        var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) ?? throw new Exception();

        var diagnosticParent =
            root.FindNode(context.Span)
               .Parent;

        var originalInvocationExpression = diagnosticParent.Parent as InvocationExpressionSyntax;

        var closeParenToken =
            ArgumentList(
                Token(SyntaxKind.OpenParenToken),
                originalInvocationExpression.ArgumentList.Arguments,
                Token(
                    TriviaList(),
                    SyntaxKind.CloseParenToken,
                    TriviaList(LineFeed)));

        var modifiedInvocationExpression =
            originalInvocationExpression
               .ReplaceNode(originalInvocationExpression.ArgumentList, closeParenToken);

        var whitespaceTrivia =
            modifiedInvocationExpression
               .GetLeadingTrivia()
               .Last(syntaxTrivia => syntaxTrivia.IsKind(SyntaxKind.WhitespaceTrivia));

        var expressionStatementSyntax =
            ExpressionStatement(
                    InvocationExpression(
                            MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    modifiedInvocationExpression
                                       .WithArgumentList(closeParenToken),
                                    IdentifierName("DisposeWith"))
                               .WithOperatorToken(
                                    Token(
                                        TriviaList(
                                            whitespaceTrivia,
                                            Whitespace("   ")), // HACK: [rlittlesii: March 25, 2023] Whitespace alignment is hard!
                                        SyntaxKind.DotToken,
                                        TriviaList())))
                       .WithArgumentList(
                            ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            IdentifierName(
                                                "Garbage")))) // TODO: [rlittlesii: March 25, 2023] Look for a CompositeDisposable use it's identifier name
                               .WithOpenParenToken(Token(SyntaxKind.OpenParenToken))
                               .WithCloseParenToken(Token(SyntaxKind.CloseParenToken))))
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        var node = root.ReplaceNode(originalInvocationExpression, expressionStatementSyntax.Expression);
        return context.Document.WithSyntaxRoot(node);
    }

    private const string Title = "Add DisposeWith to subscription";
}