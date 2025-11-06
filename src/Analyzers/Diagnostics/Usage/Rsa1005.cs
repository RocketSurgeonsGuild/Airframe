using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;

public class Rsa1005 : Rsa1000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA1005];

    // Common Rx methods that have scheduler overloads

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context) => AnalyzeInvocation(context);

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var symbolInfo = ModelExtensions.GetSymbolInfo(context.SemanticModel, invocation);

        if (symbolInfo.Symbol is not IMethodSymbol method)
        {
            return;
        }

        // Check if this is a method we care about
        if (!SchedulerAwareMethods.Contains(method.Name))
        {
            return;
        }

        // Check if the method is from System.Reactive namespace
        if (!IsReactiveExtensionsMethod(method))
        {
            return;
        }

        // Check if this method already has an IScheduler parameter
        if (HasSchedulerParameter(method))
        {
            return;
        }

        // Check if there's an overload with IScheduler available
        if (!HasSchedulerOverload(method, context.SemanticModel))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            RSA1005,
            GetMethodNameLocation(invocation),
            method.Name);

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

    private static bool HasSchedulerParameter(IMethodSymbol method) => method.Parameters.Any(p => IsSchedulerType(p.Type));

    private static bool HasSchedulerOverload(IMethodSymbol method, SemanticModel semanticModel)
    {
        // Get all methods with the same name in the same type
        var allMethods = method.ContainingType.GetMembers(method.Name)
           .OfType<IMethodSymbol>()
           .Where(m => m.IsStatic == method.IsStatic);

        // Check if any overload has a scheduler parameter
        return allMethods.Any(m => HasSchedulerParameter(m));
    }

    private static bool IsSchedulerType(ITypeSymbol type)
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
        "TakeUntil", // time-based overload
        "SkipUntil", // time-based overload
        "StartWith", // scheduler overload exists
        "Concat", // scheduler overload exists
        "Merge", // scheduler overload exists
        "Zip", // scheduler overload exists
        "CombineLatest"); // scheduler overload exists
}