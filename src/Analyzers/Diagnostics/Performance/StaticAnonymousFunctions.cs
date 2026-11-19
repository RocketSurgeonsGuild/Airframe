using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;

/// <summary>Shares static anonymous-function eligibility between the analyzer and its code fix.</summary>
internal static class StaticAnonymousFunctions
{
    internal static bool CanMakeStatic(SemanticModel model, AnonymousFunctionExpressionSyntax function, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (((CSharpParseOptions)function.SyntaxTree.Options).LanguageVersion < LanguageVersion.CSharp9
            || function.Modifiers.Any(SyntaxKind.StaticKeyword)
            || function.ContainsDiagnostics
            || function is LambdaExpressionSyntax { AttributeLists.Count: > 0 }
            || model.GetOperation(function, cancellationToken) is not IAnonymousFunctionOperation)
        {
            return false;
        }

        var flow = model.AnalyzeDataFlow(function);
        if (flow is not { Succeeded: true })
        {
            return false;
        }

        // Nested functions may capture parameters or locals declared inside this function. That
        // does not prevent this function itself from being static. Captures in sibling functions
        // (CapturedOutside) do not determine eligibility here either.
        return !flow.CapturedInside.Any(symbol => !DeclaredInside(symbol, function))
            && !flow.UsedLocalFunctions.Any(symbol => !symbol.IsStatic && !DeclaredInside(symbol, function));
    }

    private static bool DeclaredInside(ISymbol symbol, SyntaxNode function) => symbol.DeclaringSyntaxReferences
       .Any(reference => reference.SyntaxTree == function.SyntaxTree && function.Span.Contains(reference.Span));
}
