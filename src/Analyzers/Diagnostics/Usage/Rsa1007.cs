using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage
{
    /// <summary>
    /// Represents a diagnostic for <see cref="Descriptions.RSA1007"/>.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Rsa1007 : DiagnosticAnalyzer
    {
        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [RSA1007];

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocationExpression)
            {
                return;
            }

            var semanticModel = context.SemanticModel;
            var expressionType = semanticModel.GetTypeInfo(invocationExpression.Expression).Type;

            if (expressionType == null ||
                (expressionType.TypeKind != TypeKind.Delegate &&
                    expressionType.Name != "Func" &&
                    expressionType.Name != "Action"))
            {
                return;
            }

            if (invocationExpression.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Invoke" })
            {
                return;
            }

            context.ReportDiagnostic(
                Diagnostic.Create(
                    RSA1007,
                    invocationExpression.GetLocation(),
                    invocationExpression.ToFullString()));
        }
    }
}