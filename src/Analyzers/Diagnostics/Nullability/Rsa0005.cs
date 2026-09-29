using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0005"/>.
/// </summary>
/// <remarks>
/// Reports on <c>?.</c>, <c>??</c>, and <c>??=</c> whose left-hand operand is a reference type with
/// a nullable-annotated declared type (<see cref="NullableAnnotation.Annotated"/>) but a flow state
/// the compiler has already narrowed to <see cref="NullableFlowState.NotNull"/> - an earlier null
/// check, a narrowing assignment, or a pattern match already resolved it. An operand whose declared
/// type was never nullable is out of scope entirely (its <see cref="NullableAnnotation"/> is not
/// <c>Annotated</c>), as is one in a nullable-oblivious region, where flow state is not
/// meaningfully tracked at all. This is the mirror image of CS8602 (possible null reference), which
/// fires when flow analysis says an access <i>might</i> be unsafe; this rule fires where an operator
/// exists to guard against null and flow analysis says the guard cannot ever trigger.
/// </remarks>
/// <remarks>
/// A nullable <em>value</em> type operand (e.g. <c>int?</c>) is explicitly excluded via an
/// <see cref="ITypeSymbol.IsReferenceType"/> check. <c>?.</c>/<c>??</c>/<c>??=</c> on a
/// <c>Nullable&lt;T&gt;</c> is not defensive noise the way it is for a reference type: it is the
/// only way to unwrap the value, and "remove the operator" - this rule's own remediation advice -
/// would leave code that no longer compiles (<c>int? x = 1; return x ?? 0;</c> cannot become
/// <c>return x;</c> for an <c>int</c>-returning member). RSA0007 and RSA0009 both already gate on
/// <c>IsReferenceType</c> for the equivalent reason; this rule originally did not, and reported a
/// nullable value type operand as if it were reference-type noise.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0005 : Rsa0000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0005];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var operand = context.Node switch
        {
            ConditionalAccessExpressionSyntax conditionalAccess => conditionalAccess.Expression,
            BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.CoalesceExpression) => binary.Left,
            AssignmentExpressionSyntax assignment when assignment.IsKind(SyntaxKind.CoalesceAssignmentExpression) => assignment.Left,
            var _ => null
        };

        if (operand is null)
        {
            return;
        }

        var typeInfo = context.SemanticModel.GetTypeInfo(operand, context.CancellationToken);

        if (typeInfo.Type is not { IsReferenceType: true } ||
            typeInfo.Nullability.Annotation != NullableAnnotation.Annotated ||
            typeInfo.Nullability.FlowState != NullableFlowState.NotNull)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA0005, context.Node.GetLocation(), operand.ToString()));
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() =>
    [
        SyntaxKind.ConditionalAccessExpression,
        SyntaxKind.CoalesceExpression,
        SyntaxKind.CoalesceAssignmentExpression
    ];
}
