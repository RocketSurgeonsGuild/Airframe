using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA3005"/>.
/// </summary>
/// <remarks>
/// This is a different check than <see cref="Rsa3001"/>'s "is there any DisposeWith at all" — this
/// rule only inspects DisposeWith calls that already exist inside a WhenActivated block and asks
/// whether they target that block's own disposable. A DisposeWith call is never a bare, undisposed
/// statement, so RSA3001 never fires on the same call this rule reports.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa3005 : Rsa3000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA3005];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is not MemberAccessExpressionSyntax { Name.Identifier.Text: "DisposeWith" } memberAccess
         || invocation.ArgumentList.Arguments.Count != 1)
        {
            return;
        }

        var whenActivatedLambda = FindEnclosingWhenActivatedLambda(invocation);
        if (whenActivatedLambda is null)
        {
            return;
        }

        var parameterSyntax = GetLambdaParameter(whenActivatedLambda);
        if (parameterSyntax is null)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(parameterSyntax) is not { } parameterSymbol)
        {
            return;
        }

        var argumentExpression = invocation.ArgumentList.Arguments[0].Expression;
        var argumentSymbol = context.SemanticModel.GetSymbolInfo(argumentExpression).Symbol;

        if (SymbolEqualityComparer.Default.Equals(argumentSymbol, parameterSymbol))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                RSA3005,
                memberAccess.Name.GetLocation(),
                argumentExpression.ToString(),
                parameterSymbol.Name));
    }

    /// <summary>
    /// Walks outward from a DisposeWith invocation looking for the nearest ancestor lambda that is
    /// itself the sole argument to a <c>WhenActivated(...)</c> invocation. This rule only fires
    /// inside WhenActivated blocks, so no match means "not our concern" rather than "no diagnostic".
    /// </summary>
    private static LambdaExpressionSyntax? FindEnclosingWhenActivatedLambda(SyntaxNode node) =>
        node
           .Ancestors()
           .OfType<LambdaExpressionSyntax>()
           .FirstOrDefault(IsWhenActivatedArgument);

    private static bool IsWhenActivatedArgument(LambdaExpressionSyntax lambda) =>
        lambda.Parent is ArgumentSyntax
        {
            Parent: ArgumentListSyntax
            {
                Parent: InvocationExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "WhenActivated" }
                }
            }
        };

    private static ParameterSyntax? GetLambdaParameter(LambdaExpressionSyntax lambda) => lambda switch
    {
        SimpleLambdaExpressionSyntax simple => simple.Parameter,
        ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized.ParameterList.Parameters.FirstOrDefault(),
        var _ => null
    };
}
