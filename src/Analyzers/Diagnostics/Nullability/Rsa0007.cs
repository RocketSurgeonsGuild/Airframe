using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA0007"/>.
/// </summary>
/// <remarks>
/// Registers on both the C# 7.1 default literal (<c>default</c>) and the classic
/// <c>default(T)</c> form, and reports whenever the expression's converted type - the type the
/// surrounding context actually needs, not necessarily <c>T</c> itself - is a nullable-annotated
/// reference type. An unconstrained generic type parameter is not a reference type by Roslyn's own
/// definition unless constrained to <c>class</c>, so <c>default</c> targeting one is left alone, the
/// same exclusion RSA0009 makes for the same reason.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa0007 : Rsa0000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA0007];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var convertedType = context.SemanticModel.GetTypeInfo(context.Node, context.CancellationToken).ConvertedType;

        if (convertedType is null ||
            convertedType.NullableAnnotation != NullableAnnotation.Annotated ||
            !convertedType.IsReferenceType)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(RSA0007, context.Node.GetLocation(), convertedType.ToDisplayString()));
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() =>
    [
        SyntaxKind.DefaultLiteralExpression,
        SyntaxKind.DefaultExpression
    ];
}
