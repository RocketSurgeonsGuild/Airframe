using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Usage;

/// <summary>
/// Represents a code fix for <see cref="Descriptions.RSA1008"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public class Rsa1008Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds => [RSA1008.Id];

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        if (root?.FindNode(diagnosticSpan).FirstAncestorOrSelf<PropertyDeclarationSyntax>() is not { } propertyDeclaration)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: ct => UseRaiseAndSetIfChangedAsync(context.Document, propertyDeclaration, ct),
                equivalenceKey: nameof(Rsa1008Fix)),
            diagnostic);
    }

    private static async Task<Document> UseRaiseAndSetIfChangedAsync(
        Document document,
        PropertyDeclarationSyntax propertyDeclaration,
        CancellationToken cancellationToken)
    {
        var setAccessor = propertyDeclaration.AccessorList?.Accessors
           .FirstOrDefault(accessor => accessor.IsKind(SyntaxKind.SetAccessorDeclaration));

        if (setAccessor is null)
        {
            return document;
        }

        var isArrowBodied = setAccessor.ExpressionBody is not null;

        var assignment = isArrowBodied
            ? setAccessor.ExpressionBody?.Expression as AssignmentExpressionSyntax
            : (setAccessor.Body?.Statements[0] as ExpressionStatementSyntax)?.Expression as AssignmentExpressionSyntax;

        if (assignment is not { Left: IdentifierNameSyntax field })
        {
            return document;
        }

        var raiseAndSetIfChanged =
            InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ThisExpression(),
                        IdentifierName("RaiseAndSetIfChanged")))
               .WithArgumentList(
                    ArgumentList(
                        SeparatedList(
                        [
                            Argument(field).WithRefKindKeyword(Token(SyntaxKind.RefKeyword)),
                            Argument(IdentifierName("value"))
                        ])));

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        if (isArrowBodied)
        {
            editor.ReplaceNode(setAccessor, setAccessor.WithExpressionBody(ArrowExpressionClause(raiseAndSetIfChanged)));
        }
        else
        {
            editor.ReplaceNode(setAccessor.Body!.Statements[0], ExpressionStatement(raiseAndSetIfChanged));
        }

        return editor.GetChangedDocument();
    }

    private const string Title = "Use RaiseAndSetIfChanged";
}
