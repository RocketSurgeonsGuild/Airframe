using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
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

        // Check if this is a method we care about
        if (!SchedulerAwareMethods.Contains(actualMethod.Name))
        {
            return;
        }

        // Check if the method is from System.Reactive namespace
        if (!IsReactiveExtensionsMethod(actualMethod))
        {
            return;
        }

        // Check if this method already has an IScheduler parameter
        if (HasSchedulerParameter(actualMethod))
        {
            return;
        }

        // Check if there's an overload with IScheduler available
        if (!HasSchedulerOverload(actualMethod))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            RSA1005,
            GetMethodNameLocation(invocation),
            actualMethod.Name);

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsReactiveExtensionsMethod(IMethodSymbol method)
    {
        // Check if the method is from System.Reactive namespaces
        var containingNamespace = method.ContainingNamespace?.ToDisplayString();
        return containingNamespace != null &&
            (containingNamespace.StartsWith("System.Reactive") ||
                containingNamespace.StartsWith("System.Observable"));
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

        // Check for IScheduler interface
        if (type.Name == "IScheduler" &&
            type.ContainingNamespace?.ToDisplayString() == "System.Reactive.Concurrency")
        {
            return true;
        }

        // Check implemented interfaces
        return type.AllInterfaces.Any(i =>
            i.Name == "IScheduler" &&
            i.ContainingNamespace?.ToDisplayString() == "System.Reactive.Concurrency");
    }

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:Code should not contain multiple whitespace in a row", Justification = "Looks better.")]
    private static Location GetMethodNameLocation(InvocationExpressionSyntax invocation) => invocation.Expression switch
    {
        MemberAccessExpressionSyntax memberAccess => memberAccess.Name.GetLocation(),
        IdentifierNameSyntax identifier           => identifier.GetLocation(),
        _                                         => invocation.GetLocation()
    };

    private static readonly ImmutableHashSet<string> SchedulerAwareMethods = ImmutableHashSet.Create(
        "Throttle",
        "Debounce",
        "Sample",
        "Buffer",
        "Window",
        "Delay",
        "DelaySubscription",
        "Timeout",
        "Timer",
        "Interval",
        "ObserveOn",
        "SubscribeOn",
        "TimeInterval",
        "Timestamp",
        "TakeUntil",
        "SkipUntil",
        "StartWith",
        "Concat",
        "Merge",
        "Zip",
        "CombineLatest");
}