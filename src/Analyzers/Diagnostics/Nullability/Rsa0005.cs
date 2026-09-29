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
/// Reports on <c>?.</c>, <c>??</c>, and <c>??=</c> whose left-hand operand has a nullable-annotated
/// declared type (<see cref="NullableAnnotation.Annotated"/>) but a flow state the compiler has
/// already narrowed to <see cref="NullableFlowState.NotNull"/> - an earlier null check, a narrowing
/// assignment, or a pattern match already resolved it. An operand whose declared type was never
/// nullable is out of scope entirely (its <see cref="NullableAnnotation"/> is not
/// <c>Annotated</c>), as is one in a nullable-oblivious region, where flow state is not
/// meaningfully tracked at all. This is the mirror image of CS8602 (possible null reference), which
/// fires when flow analysis says an access <i>might</i> be unsafe; this rule fires where an operator
/// exists to guard against null and flow analysis says the guard cannot ever trigger.
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

        var nullability = context.SemanticModel.GetTypeInfo(operand, context.CancellationToken).Nullability;

        if (nullability.Annotation != NullableAnnotation.Annotated || nullability.FlowState != NullableFlowState.NotNull)
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
