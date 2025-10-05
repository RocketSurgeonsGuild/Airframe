using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;
using static Rocket.Surgery.Airframe.Analyzers.Category;

namespace Rocket.Surgery.Airframe.Analyzers;

internal static class Descriptions
{
    private static readonly ConcurrentDictionary<Category, string> CategoryMap = new();

    public static DiagnosticDescriptor RSA1001 { get; } =
        new(
            "RSA1001",
            "Use expression lambda overload",
            "Use expression lambda overload for property {0}",
            CategoryMap.GetOrAdd(Usage, category => category.ToString()),
            Warning,
            true);

    public static DiagnosticDescriptor RSA1002 { get; } =
        new(
            "RSA1002",
            "Unsupported expression type.",
            "Provide a well-formed lambda expression",
            CategoryMap.GetOrAdd(Usage, category => category.ToString()),
            Error,
            true);

    public static DiagnosticDescriptor RSA1003 { get; } =
        new(
            "RSA1003",
            "Out parameter assignment",
            "Use the out parameter overload",
            CategoryMap.GetOrAdd(Usage, category => category.ToString()),
            Error,
            true);

    public static DiagnosticDescriptor RSA1004 { get; } =
        new(
            "RSA1004",
            "Unsupported expression missing member access prefix.",
            "Provide a well-formed lambda expression",
            CategoryMap.GetOrAdd(Usage, category => category.ToString()),
            Error,
            true);

    public static DiagnosticDescriptor RSA1006 { get; } =
        new(
            "RSA1006",
            "Multiple attempts to subscribe on a thread scheduler",
            "SubscribeOn only supports a single use per pipeline",
            CategoryMap.GetOrAdd(Usage, category => category.ToString()),
            Info,
            true);

    public static DiagnosticDescriptor RSA1007 { get; } = new DiagnosticDescriptor(
        "RSA1007",
        "Use Invoke method for function calls",
        "Use the Invoke() method to call functions instead of using parentheses: '{0}'",
        CategoryMap.GetOrAdd(Usage, category => category.ToString()),
        Warning,
        true,
        "Functions should be called using the .Invoke() method rather than parentheses.");

    public static DiagnosticDescriptor RSA3001 { get; } =
        new(
            "RSA3001",
            "Subscription not disposed",
            "Consider use of DisposeWith to clean up subscriptions",
            CategoryMap.GetOrAdd(Performance, category => category.ToString()),
            Warning,
            true);

    public static DiagnosticDescriptor RSA3002 { get; } =
        new(
            "RSA3002",
            "Lambda expression can be made static",
            "Lambda expression can be made static to prevent accidental variable capture",
            CategoryMap.GetOrAdd(Performance, category => category.ToString()),
            Warning,
            true,
            "Lambda expressions that don't capture local variables or instance state can be marked as static.");
}