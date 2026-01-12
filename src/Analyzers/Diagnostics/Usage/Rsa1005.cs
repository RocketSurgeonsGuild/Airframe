using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA1005"/>.
/// </summary>
public class Rsa1005 : Rsa1000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA1005];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context) => AnalyzeInvocation(context);

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);

        if (symbolInfo.Symbol is not IMethodSymbol method)
        {
            return;
        }

        // Handle extension methods
        var actualMethod = method.ReducedFrom ?? method;

        // Check if the method is from System.Reactive namespace
        if (!IsReactiveExtensionsMethod(actualMethod))
        {
            return;
        }

        // Check if this method already has an IScheduler parameter
        var schedulerParameter = actualMethod.Parameters.FirstOrDefault(p => IsSchedulerType(p.Type));
        if (schedulerParameter != null)
        {
            // If it has a scheduler parameter, check if it's optional and if it was provided
            if (schedulerParameter.IsOptional)
            {
                var isProvided = false;
                var operation = context.SemanticModel.GetOperation(invocation) as IInvocationOperation;
                if (operation != null)
                {
                    isProvided = operation.Arguments.Any(arg =>
                        arg.Parameter?.OriginalDefinition.Equals(schedulerParameter.OriginalDefinition, SymbolEqualityComparer.Default) == true &&
                        arg.ArgumentKind != ArgumentKind.DefaultValue);
                }

                if (!isProvided)
                {
                    ReportDiagnostic(context, invocation, actualMethod);
                }
            }

            return;
        }

        // Check if there's an overload with IScheduler available
        if (HasSchedulerOverload(actualMethod))
        {
            ReportDiagnostic(context, invocation, actualMethod);
        }
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        var diagnostic = Diagnostic.Create(RSA1005, GetMethodNameLocation(invocation), method.Name);
        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsReactiveExtensionsMethod(IMethodSymbol method)
    {
        // Check if the method has an IObservable<T> parameter
        if (method.Parameters.Any(p => IsObservableType(p.Type)))
        {
            return true;
        }

        // Check if the method is from System.Reactive namespaces
        var containingNamespace = method.ContainingNamespace?.ToDisplayString();
        if (containingNamespace == null)
        {
            return false;
        }

        return containingNamespace.StartsWith("System.Reactive") ||
               containingNamespace.StartsWith("System.Observable") ||
               containingNamespace.StartsWith("DynamicData") ||
               method.ContainingAssembly.Name.Contains("DynamicData");
    }

    private static bool IsObservableType(ITypeSymbol? type)
    {
        if (type == null)
        {
            return false;
        }

        if (type.Name == "IObservable" && type.ContainingNamespace?.ToDisplayString() == "System")
        {
            return true;
        }

        return type.AllInterfaces.Any(i => i.Name == "IObservable" && i.ContainingNamespace?.ToDisplayString() == "System");
    }

    private static bool HasSchedulerParameter(IMethodSymbol method) => method.Parameters.Any(parameterSymbol => IsSchedulerType(parameterSymbol.Type));

    private static bool HasSchedulerOverload(IMethodSymbol method)
    {
        // Get all methods with the same name in the same type
        var allMethods = method.ContainingType.GetMembers(method.Name)
           .OfType<IMethodSymbol>()
           .Where(methodSymbol => methodSymbol.IsStatic == method.IsStatic);

        // Check if any overload has a scheduler parameter
        return allMethods.Any(m => HasSchedulerParameter(m));
    }

    private static bool IsSchedulerType(ITypeSymbol? type)
    {
        if (type == null)
        {
            return false;
        }

        // Handle nullable types
        if (type is INamedTypeSymbol namedType && namedType.IsGenericType && namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
        {
            return IsSchedulerType(namedType.TypeArguments[0]);
        }

        // Check for IScheduler interface
        if (type.Name == "IScheduler" &&
            (type.ContainingNamespace?.ToDisplayString() == "System.Reactive.Concurrency" ||
             type.ContainingNamespace?.ToDisplayString() == "System.Reactive"))
        {
            return true;
        }

        // Check implemented interfaces
        return type.AllInterfaces.Any(i =>
            i.Name == "IScheduler" &&
            (i.ContainingNamespace?.ToDisplayString() == "System.Reactive.Concurrency" ||
             i.ContainingNamespace?.ToDisplayString() == "System.Reactive"));
    }

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:Code should not contain multiple whitespace in a row", Justification = "Looks better.")]
    private static Location GetMethodNameLocation(InvocationExpressionSyntax invocation) => invocation.Expression switch
    {
        MemberAccessExpressionSyntax memberAccess => memberAccess.Name.GetLocation(),
        IdentifierNameSyntax identifier => identifier.GetLocation(),
        var _ => invocation.GetLocation(),
    };
}