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

            // Skip if this is already using .Invoke() syntax
            if (invocationExpression.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Invoke" })
            {
                return;
            }

            // Look at the variable declaration to determine if it's a delegate/function
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
            // First, check if this is a simple identifier invocation (like 'func()')
            if (invocationExpression.Expression is IdentifierNameSyntax identifierName)
            {
                // Look up the identifier in the parent block to see if it was declared as a delegate
                string variableName = identifierName.Identifier.ValueText;
                var enclosingScope = invocationExpression.Ancestors().OfType<BlockSyntax>().FirstOrDefault();

                if (enclosingScope != null)
                {
                    // Look for local variables with lambda assignments
                    foreach (var statement in enclosingScope.Statements)
                    {
                        if (statement is LocalDeclarationStatementSyntax localDeclaration)
                        {
                            foreach (var variable in localDeclaration.Declaration.Variables)
                            {
                                if (variable.Identifier.ValueText == variableName &&
                                    variable.Initializer?.Value is LambdaExpressionSyntax)
                                {
                                    return true;
                                }

                                // Also check for variable declarations with Func or Action types
                                var declarationType = localDeclaration.Declaration.Type.ToString();
                                if (variable.Identifier.ValueText == variableName &&
                                    (declarationType.StartsWith("Func") ||
                                        declarationType.StartsWith("Action") ||
                                        declarationType.EndsWith("Delegate")))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                // Check for class fields or properties
                var classDeclaration = invocationExpression.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
                if (classDeclaration != null)
                {
                    // Look for fields with lambda expressions
                    foreach (var member in classDeclaration.Members)
                    {
                        if (member is FieldDeclarationSyntax fieldDeclaration)
                        {
                            foreach (var variable in fieldDeclaration.Declaration.Variables)
                            {
                                if (variable.Identifier.ValueText == variableName &&
                                    variable.Initializer?.Value is LambdaExpressionSyntax)
                                {
                                    return true;
                                }

                                // Also check type names for field declarations
                                var fieldType = fieldDeclaration.Declaration.Type.ToString();
                                if (variable.Identifier.ValueText == variableName &&
                                    (fieldType.StartsWith("Func") ||
                                        fieldType.StartsWith("Action") ||
                                        fieldType.EndsWith("Delegate")))
                                {
                                    return true;
                                }
                            }
                        }
                        else if (member is PropertyDeclarationSyntax propertyDeclaration)
                        {
                            if (propertyDeclaration.Identifier.ValueText == variableName &&
                                propertyDeclaration.Initializer?.Value is LambdaExpressionSyntax)
                            {
                                return true;
                            }

                            // Check property types
                            var propertyType = propertyDeclaration.Type.ToString();
                            if (propertyDeclaration.Identifier.ValueText == variableName &&
                                (propertyType.StartsWith("Func") ||
                                    propertyType.StartsWith("Action") ||
                                    propertyType.EndsWith("Delegate")))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // Check for member access that might be a delegate (object.Method() where Method is a delegate)
            if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                // This would need more complex analysis, but we can look for common patterns
                // like event handlers that are often delegates
                string memberName = memberAccess.Name.Identifier.ValueText;
                if (memberName.EndsWith("Handler") ||
                    memberName.EndsWith("Callback") ||
                    memberName.EndsWith("Action") ||
                    memberName.EndsWith("Func"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}