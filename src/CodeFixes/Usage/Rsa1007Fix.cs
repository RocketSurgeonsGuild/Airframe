using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Usage
{
    /// <summary>
    /// Represents a code fix for <see cref="Descriptions.RSA1007"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    [Shared]
    public class Rsa1007Fix : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => [RSA1007.Id];

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            if (root?.FindNode(diagnosticSpan) is not InvocationExpressionSyntax invocation)
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Use Invoke() method",
                    createChangedDocument: ct => ConvertToInvokeMethodAsync(context.Document, invocation, ct),
                    equivalenceKey: nameof(Rsa1007Fix)),
                diagnostic);
        }

        private async Task<Document> ConvertToInvokeMethodAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            // Convert func() to func.Invoke()
            var arguments = invocation.ArgumentList;
            var memberAccess = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                invocation.Expression,
                IdentifierName("Invoke"));

            var newInvocation = InvocationExpression(memberAccess, arguments);

            editor.ReplaceNode(invocation, newInvocation);
            return editor.GetChangedDocument();
        }
    }
}