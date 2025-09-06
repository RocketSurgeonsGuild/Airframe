using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA1006"/>.
/// </summary>
public class Rsa1006 : Rsa1000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA1006];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return;
        }

        var identifierNames =
            invocationExpressionSyntax
               .DescendantNodes()
               .OfType<IdentifierNameSyntax>()
               .Where(identifierNameSyntax => identifierNameSyntax.Identifier.Text == "SubscribeOn")
               .ToArray();

        if (identifierNames.Length <= 1)
        {
            return;
        }

        foreach (var identifierName in identifierNames)
        {
            context.ReportDiagnostic(Diagnostic.Create(RSA1006, identifierName.GetLocation()));
        }
    }
}