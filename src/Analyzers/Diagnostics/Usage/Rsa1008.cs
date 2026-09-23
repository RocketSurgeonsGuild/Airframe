using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA1008"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa1008 : Rsa1000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA1008];

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.PropertyDeclaration];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

        if (!HasDirectFieldAssignmentSetter(propertyDeclaration))
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(propertyDeclaration) is not IPropertySymbol propertySymbol)
        {
            return;
        }

        if (!DerivesFromReactiveObject(propertySymbol.ContainingType))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(RSA1008, propertyDeclaration.Identifier.GetLocation(), propertyDeclaration.Identifier.Text));
    }

    /// <summary>
    /// Determines, syntax-only, whether the property has an explicit setter whose entire body is
    /// a single plain assignment of <c>value</c> to a backing field (e.g. <c>field = value;</c> or
    /// <c>=&gt; field = value;</c>). Auto-properties (no explicit setter), <c>init</c>-only setters,
    /// multi-statement setters, and setters that already call a method (RaiseAndSetIfChanged and
    /// friends) never match this shape, so this is the entire false-positive guard for this rule.
    /// </summary>
    private static bool HasDirectFieldAssignmentSetter(PropertyDeclarationSyntax propertyDeclaration)
    {
        var setAccessor = propertyDeclaration.AccessorList?.Accessors
           .FirstOrDefault(accessor => accessor.IsKind(SyntaxKind.SetAccessorDeclaration));

        if (setAccessor is null)
        {
            return false;
        }

        var assignment = GetSingleAssignment(setAccessor);

        return assignment is { } expression
         && expression.IsKind(SyntaxKind.SimpleAssignmentExpression)
         && expression.Left is IdentifierNameSyntax
         && expression.Right is IdentifierNameSyntax { Identifier.Text: "value" };
    }

    private static AssignmentExpressionSyntax? GetSingleAssignment(AccessorDeclarationSyntax setAccessor)
    {
        if (setAccessor.ExpressionBody is { Expression: AssignmentExpressionSyntax arrowAssignment })
        {
            return arrowAssignment;
        }

        if (setAccessor.Body is { } body
         && body.Statements.Count == 1
         && body.Statements[0] is ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax blockAssignment })
        {
            return blockAssignment;
        }

        return null;
    }

    private static bool DerivesFromReactiveObject(INamedTypeSymbol? type)
    {
        for (var current = type?.BaseType; current is not null; current = current.BaseType)
        {
            if (current.Name == "ReactiveObject")
            {
                return true;
            }
        }

        return false;
    }
}
