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
/// <para>
/// Counts every null-forgiving operator (<c>!</c>) - a <see cref="SyntaxKind.SuppressNullableWarningExpression"/>
/// - within a method, constructor, property, indexer, operator, or conversion operator
/// declaration, and reports once on the member when the count exceeds the configured threshold. A
/// null-forgiving operator nested inside a local function or lambda declared within the member is
/// still counted against that member, rather than tracked separately: the density this rule cares
/// about is per author unit of code, not per syntactic scope. For a property, both the accessor
/// bodies (or the expression body) <em>and</em> the initializer are scanned - a property can have
/// both at once (<c>public string S { get; set; } = a!.Trim();</c>), and a member with all its
/// suppressions living in the initializer is exactly as dense as one with them in the accessors.
/// </para>
/// <para>
/// A <c>!</c> immediately to the left of <c>is</c> or an <c>is</c> pattern (e.g. <c>x! is string</c>)
/// is excluded from the count. Confirmed against the built-in analyzer's decompiled source: the
/// built-in IDE0080 ("Remove unnecessary suppression operator") is registered <i>only</i> for that
/// exact shape - <c>CSharpRemoveConfusingSuppressionDiagnosticAnalyzer</c> registers solely on
/// <see cref="SyntaxKind.IsExpression"/> and <see cref="SyntaxKind.IsPatternExpression"/>, and fires
/// only when their left-hand expression is a suppression - because a type test ignores nullability
/// entirely, so the operator suppresses nothing there and IDE0080 already reports it individually.
/// Every other <c>!</c> site - the overwhelming majority - is outside IDE0080's registration
/// entirely, so RSA0004's per-member density count and IDE0080's per-site check do not otherwise
/// overlap: density can be real with zero IDE0080 hits (every use individually justified), and an
/// IDE0080 hit can exist on a member nowhere near the density threshold.
/// </para>
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
        var (name, nameLocation, fragments) = context.Node switch
        {
            MethodDeclarationSyntax method =>
                (method.Identifier.Text, method.Identifier.GetLocation(), new SyntaxNode?[] { method.Body, method.ExpressionBody }),
            ConstructorDeclarationSyntax constructor =>
                (constructor.Identifier.Text, constructor.Identifier.GetLocation(), new SyntaxNode?[] { constructor.Body, constructor.ExpressionBody }),
            PropertyDeclarationSyntax property =>
                (property.Identifier.Text, property.Identifier.GetLocation(), new SyntaxNode?[] { property.AccessorList, property.ExpressionBody, property.Initializer }),
            IndexerDeclarationSyntax indexer =>
                ("this[]", indexer.ThisKeyword.GetLocation(), new SyntaxNode?[] { indexer.AccessorList, indexer.ExpressionBody }),
            OperatorDeclarationSyntax @operator =>
                ($"operator {@operator.OperatorToken.Text}", @operator.OperatorToken.GetLocation(), new SyntaxNode?[] { @operator.Body, @operator.ExpressionBody }),
            ConversionOperatorDeclarationSyntax conversion =>
                ($"operator {conversion.Type}", conversion.OperatorKeyword.GetLocation(), new SyntaxNode?[] { conversion.Body, conversion.ExpressionBody }),
            var _ => (null, null, null)
        };

        if (name is null || fragments is null)
        {
            return;
        }

        var count = fragments
           .Where(fragment => fragment is not null)
           .SelectMany(fragment => fragment!.DescendantNodes())
           .OfType<PostfixUnaryExpressionSyntax>()
           .Count(expression =>
                expression.IsKind(SyntaxKind.SuppressNullableWarningExpression) &&
                !IsLeftOfTypeTest(expression));

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
        SyntaxKind.PropertyDeclaration,
        SyntaxKind.IndexerDeclaration,
        SyntaxKind.OperatorDeclaration,
        SyntaxKind.ConversionOperatorDeclaration
    ];

    /// <summary>
    /// Determines whether <paramref name="expression"/> is the left-hand operand of <c>is</c> or an
    /// <c>is</c> pattern - the one shape IDE0080 already reports on individually, since a type test
    /// ignores nullability and the operator suppresses nothing there.
    /// </summary>
    private static bool IsLeftOfTypeTest(ExpressionSyntax expression) => expression.Parent switch
    {
        BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.IsExpression) => binary.Left == expression,
        IsPatternExpressionSyntax isPattern => isPattern.Expression == expression,
        var _ => false
    };

    private static int GetThreshold(SyntaxNodeAnalysisContext context)
    {
        var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);

        return options.TryGetValue(ThresholdOption, out var value) &&
               int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var configured) &&
               configured >= 0
            ? configured
            : DefaultThreshold;
    }
}
