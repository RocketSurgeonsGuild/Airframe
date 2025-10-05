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

            if (invocationExpression.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Invoke" })
            {
                return;
            }

            if (IsPotentialDelegateInvocation(invocationExpression))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        RSA1007,
                        invocationExpression.GetLocation(),
                        invocationExpression.ToFullString()));
            }
        }

        private static bool IsPotentialDelegateInvocation(InvocationExpressionSyntax invocationExpression)
        {
            const string func = "Func";
            const string action = "Action";
            const string @delegate = "Delegate";
            if (invocationExpression.Expression is IdentifierNameSyntax identifierName)
            {
                var variableName = identifierName.Identifier.ValueText;
                var enclosingScope = invocationExpression.Ancestors().OfType<BlockSyntax>().FirstOrDefault();

                if (enclosingScope != null)
                {
                    foreach (var statement in enclosingScope.Statements)
                    {
                        if (statement is not LocalDeclarationStatementSyntax localDeclaration)
                        {
                            continue;
                        }

                        foreach (var variable in localDeclaration.Declaration.Variables)
                        {
                            if (variable.Identifier.ValueText == variableName &&
                                variable.Initializer?.Value is LambdaExpressionSyntax)
                            {
                                return true;
                            }

                            var declarationType = localDeclaration.Declaration.Type.ToString();
                            if (variable.Identifier.ValueText == variableName &&
                                (declarationType.StartsWith(func) ||
                                    declarationType.StartsWith(action) ||
                                    declarationType.EndsWith(@delegate)))
                            {
                                return true;
                            }
                        }
                    }
                }

                var classDeclaration = invocationExpression.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
                if (classDeclaration != null)
                {
                    foreach (var member in classDeclaration.Members)
                    {
                        switch (member)
                        {
                            case FieldDeclarationSyntax fieldDeclaration:
                            {
                                foreach (var variable in fieldDeclaration.Declaration.Variables)
                                {
                                    if (variable.Identifier.ValueText == variableName &&
                                        variable.Initializer?.Value is LambdaExpressionSyntax)
                                    {
                                        return true;
                                    }

                                    var fieldType = fieldDeclaration.Declaration.Type.ToString();
                                    if (variable.Identifier.ValueText == variableName &&
                                        (fieldType.StartsWith(func) ||
                                            fieldType.StartsWith(action) ||
                                            fieldType.EndsWith(@delegate)))
                                    {
                                        return true;
                                    }
                                }

                                break;
                            }

                            case PropertyDeclarationSyntax propertyDeclaration when propertyDeclaration.Identifier.ValueText == variableName &&
                                propertyDeclaration.Initializer?.Value is LambdaExpressionSyntax:
                                return true;

                            case PropertyDeclarationSyntax propertyDeclaration:
                            {
                                var propertyType = propertyDeclaration.Type.ToString();
                                if (propertyDeclaration.Identifier.ValueText == variableName &&
                                    (propertyType.StartsWith(func) ||
                                        propertyType.StartsWith(action) ||
                                        propertyType.EndsWith(@delegate)))
                                {
                                    return true;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccess)
            {
                return false;
            }

            var memberName = memberAccess.Name.Identifier.ValueText;
            return
                memberName.EndsWith(action) ||
                memberName.EndsWith(func) ||
                memberName.EndsWith("Handler") ||
                memberName.EndsWith("Callback");
        }
    }
}