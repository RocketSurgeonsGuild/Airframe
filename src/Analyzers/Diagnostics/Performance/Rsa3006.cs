using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA3006"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa3006 : Rsa3000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA3006];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (!ObservableFromEventSymbols.TryGetFromEventMethod(invocation, context.SemanticModel, out var method))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA3006, GetMethodNameLocation(invocation), $"Observable.{method.Name}"));
    }
}
