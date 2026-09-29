using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0004"/>.
/// </summary>
/// <remarks>
/// Counts every null-forgiving operator (<c>!</c>) - a <see cref="SyntaxKind.SuppressNullableWarningExpression"/>
/// - within a method, constructor, or property declaration's body or expression body, and reports
/// once on the member when the count exceeds the configured threshold. A null-forgiving operator
/// nested inside a local function or lambda declared within the member is still counted against
/// that member, rather than tracked separately: the density this rule cares about is per author
/// unit of code, not per syntactic scope.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0004 : Rsa0000
{
    private const int DefaultThreshold = 3;
    private const string ThresholdOption = "rsa0004_max_null_forgiving_operators";

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0004];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var (name, nameLocation, body) = context.Node switch
        {
            MethodDeclarationSyntax method => (method.Identifier.Text, method.Identifier.GetLocation(), (SyntaxNode?)method.Body ?? method.ExpressionBody),
            ConstructorDeclarationSyntax constructor => (constructor.Identifier.Text, constructor.Identifier.GetLocation(), (SyntaxNode?)constructor.Body ?? constructor.ExpressionBody),
            PropertyDeclarationSyntax property => (property.Identifier.Text, property.Identifier.GetLocation(), (SyntaxNode?)property.AccessorList ?? property.ExpressionBody),
            var _ => (null, null, null)
        };

        if (name is null || body is null)
        {
            return;
        }

        var count = body
           .DescendantNodes()
           .OfType<PostfixUnaryExpressionSyntax>()
           .Count(expression => expression.IsKind(SyntaxKind.SuppressNullableWarningExpression));

        var threshold = GetThreshold(context);

        if (count <= threshold)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA0004, nameLocation, name, count, threshold));
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() =>
    [
        SyntaxKind.MethodDeclaration,
        SyntaxKind.ConstructorDeclaration,
        SyntaxKind.PropertyDeclaration
    ];

    private static int GetThreshold(SyntaxNodeAnalysisContext context)
    {
        var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);

        return options.TryGetValue(ThresholdOption, out var value) &&
               int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var configured) &&
               configured > 0
            ? configured
            : DefaultThreshold;
    }
}
