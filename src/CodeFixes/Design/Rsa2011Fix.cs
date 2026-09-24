using System.Collections.Immutable;
using System.Composition;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Design;

/// <summary>
/// Represents a code fix for <see cref="Descriptions.RSA2011"/>.
/// </summary>
/// <remarks>
/// The inserted modifier is the one C# already applies, so the fix states the existing
/// accessibility rather than changing it.
/// </remarks>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa2011Fix))]
public class Rsa2011Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [RSA2011.Id];

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

        var member = root
           .FindToken(diagnostic.Location.SourceSpan.Start)
           .Parent?
           .AncestorsAndSelf()
           .OfType<MemberDeclarationSyntax>()
           .FirstOrDefault();

        if (member == null)
        {
            return;
        }

        var keyword = DefaultAccessibilityOf(member);
        var title = string.Format(CultureInfo.InvariantCulture, Title, SyntaxFacts.GetText(keyword));

        // The equivalence key varies with the keyword, because declaring internal and declaring
        // private are different fixes and Fix All must not treat one as the other.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: title,
                createChangedDocument: c => DeclareAsync(context.Document, root, member, keyword, c),
                equivalenceKey: title),
            diagnostic);
    }

    private static SyntaxKind DefaultAccessibilityOf(MemberDeclarationSyntax member) =>
        member.Parent is CompilationUnitSyntax or BaseNamespaceDeclarationSyntax
            ? SyntaxKind.InternalKeyword
            : SyntaxKind.PrivateKeyword;

    private static Task<Document> DeclareAsync(Document document, SyntaxNode root, MemberDeclarationSyntax member, SyntaxKind keyword, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var modifier = Token(keyword).WithTrailingTrivia(Space);
        MemberDeclarationSyntax declared;

        // The new modifier becomes the first token of the declaration, so it inherits the leading
        // trivia, meaning the indentation and any XML documentation, from whatever held it before.
        if (member.Modifiers.Count > 0)
        {
            var first = member.Modifiers[0];
            var modifiers = member
               .Modifiers
               .Replace(first, first.WithLeadingTrivia(TriviaList()))
               .Insert(0, modifier.WithLeadingTrivia(first.LeadingTrivia));

            declared = member.WithModifiers(modifiers);
        }
        else
        {
            var first = member.GetFirstToken();

            declared = member
               .ReplaceToken(first, first.WithLeadingTrivia(TriviaList()))
               .WithModifiers(TokenList(modifier.WithLeadingTrivia(first.LeadingTrivia)));
        }

        return Task.FromResult(
            document.WithSyntaxRoot(root.ReplaceNode(member, declared.WithAdditionalAnnotations(Formatter.Annotation))));
    }

    private const string Title = "Declare '{0}' explicitly";
}
