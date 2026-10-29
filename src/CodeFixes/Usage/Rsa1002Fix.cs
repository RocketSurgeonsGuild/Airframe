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
/// Represents a code fix for <see cref="Descriptions.RSA1001"/>.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa1002Fix))]
public class Rsa1002Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var ancestorsAndSelf = (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf() ?? []).ToList();
        var invocation = ancestorsAndSelf.OfType<InvocationExpressionSyntax>().First();
        var declaration = ancestorsAndSelf.OfType<SimpleLambdaExpressionSyntax>().First();
        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => Fix(context, invocation, declaration, c),
                equivalenceKey: RSA1002.Id + RSA1002.Title + "BindTo"),
            diagnostic);
    }

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [RSA1002.Id];

    private static async Task<Document> Fix(
        CodeFixContext context,
        InvocationExpressionSyntax invocation,
        SimpleLambdaExpressionSyntax declaration,
        CancellationToken cancellationToken)
    {
        var rootAsync = await context.Document.GetSyntaxRootAsync(cancellationToken);
        if (rootAsync == null)
        {
            return context.Document;
        }

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
                                        declaration.Parameter.Identifier.Text,
                                        TriviaList(Space))))
                           .WithArrowToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.EqualsGreaterThanToken,
                                    TriviaList(Space)))
                           .WithExpressionBody(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(declaration.Parameter.Identifier),
                                    IdentifierName(declaration.Body.GetLastToken()))))
                }));

        var changed = rootAsync.ReplaceNode(invocation.ArgumentList, arguments);
        return context.Document.WithSyntaxRoot(changed);
    }

    private const string Title = "Add required member access prefix on expression lambda";
}