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
/// Represents a code fix for <see cref="Descriptions.RSA1003"/>.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa1003Fix))]
public class Rsa1003Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the type declaration identified by the diagnostic.
        var ancestorsAndSelf = (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf() ?? []).ToList();
        var invocation = ancestorsAndSelf.OfType<InvocationExpressionSyntax>().First();
        var declaration = ancestorsAndSelf.OfType<SimpleLambdaExpressionSyntax>().First();
        var variableDeclaration = ancestorsAndSelf.OfType<LocalDeclarationStatementSyntax>().First();

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => Fix(context.Document, variableDeclaration, invocation, declaration, c),
                equivalenceKey: RSA1003.Id + RSA1003.Title),
            diagnostic);
    }

    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [RSA1003.Id];

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    private static async Task<Document> Fix(
        Document document,
        LocalDeclarationStatementSyntax declarationSyntax,
        InvocationExpressionSyntax invocation,
        SimpleLambdaExpressionSyntax declaration,
        CancellationToken cancellationToken)
    {
        var rootAsync = await document.GetSyntaxRootAsync(cancellationToken);
        if (rootAsync == null)
        {
            return document;
        }

        var expressionSyntax = (MemberAccessExpressionSyntax)declaration.Body;

        var arguments =
            ArgumentList(
                SeparatedList<ArgumentSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        Argument(ThisExpression().WithToken(Token(SyntaxKind.ThisKeyword))),
                        Token(TriviaList(), SyntaxKind.CommaToken, TriviaList(Space)),
                        Argument(
                            InvocationExpression(IdentifierName(Identifier(TriviaList(), SyntaxKind.NameOfKeyword, "nameof", "nameof", TriviaList())))
                               .WithArgumentList(
                                    ArgumentList(SingletonSeparatedList(Argument(IdentifierName(expressionSyntax.Name.Identifier))))
                                       .WithOpenParenToken(Token(SyntaxKind.OpenParenToken))
                                       .WithCloseParenToken(Token(SyntaxKind.CloseParenToken)))),
                        Token(
                            TriviaList(),
                            SyntaxKind.CommaToken,
                            TriviaList(Space)),
                        Argument(IdentifierName("_" + IdentifierName(expressionSyntax.Name.Identifier.Text.ToLowerInvariant())))
                           .WithRefOrOutKeyword(Token(TriviaList(), SyntaxKind.OutKeyword, TriviaList(Space)))
                           .WithRefKindKeyword(Token(TriviaList(), SyntaxKind.OutKeyword, TriviaList(Space)))
                    }));

        var invocationExpressionSyntax = ExpressionStatement(invocation.ReplaceNode(invocation.ArgumentList, arguments)).AncestorsAndSelf();
        var changed = rootAsync.ReplaceNode(declarationSyntax, invocationExpressionSyntax);
        return document.WithSyntaxRoot(changed);
    }

    private const string Title = "Fix out parameter assignment";
}