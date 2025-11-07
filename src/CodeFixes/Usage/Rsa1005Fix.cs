using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa1005Fix))]
[Shared]
public class Rsa1005Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("RSA1005");

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.FirstOrDefault(d => d.Id == RSA1005.Id);
        if (diagnostic == null)
        {
            return;
        }

        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var invocation = root.FindNode(diagnosticSpan).FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (invocation == null)
        {
            return;
        }

        // Suggest common schedulers
        var schedulers = new[]
        {
            ("TaskPoolScheduler.Default", "Use TaskPoolScheduler.Default"),
            ("Scheduler.Default", "Use Scheduler.Default"),
            ("Scheduler.CurrentThread", "Use Scheduler.CurrentThread"),
            ("TestScheduler", "Use TestScheduler (for testing)")
        };

        foreach (var (schedulerName, title) in schedulers)
        {
            var action = CodeAction.Create(
                title: title,
                createChangedDocument: c => AddSchedulerParameter(context.Document, invocation, schedulerName, c),
                equivalenceKey: schedulerName);

            context.RegisterCodeFix(action, diagnostic);
        }
    }

    private async Task<Document> AddSchedulerParameter(Document document, InvocationExpressionSyntax invocation, string schedulerName, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var argumentList = invocation.ArgumentList;

        // Add scheduler as the last argument
        var schedulerArgument = SyntaxFactory.Argument(
            SyntaxFactory.IdentifierName(schedulerName));

        var newArgumentList = argumentList.Arguments.Count == 0
            ? argumentList.WithArguments(SyntaxFactory.SingletonSeparatedList(schedulerArgument))
            : argumentList.WithArguments(argumentList.Arguments.Add(schedulerArgument));

        var newInvocation = invocation.WithArgumentList(newArgumentList);
        var newRoot = root.ReplaceNode(invocation, newInvocation);

        return document.WithSyntaxRoot(newRoot);
    }
}