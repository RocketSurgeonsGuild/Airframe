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

    public static DiagnosticDescriptor RSA1005 { get; } = new(
        id: "RSA1005",
        title: "Consider specifying a scheduler for better control over execution timing",
        messageFormat: "Method '{0}' has an overload that accepts an IScheduler parameter. Consider using it for better testability and control over execution timing.",
        CategoryMap.GetOrAdd(Usage, category => category.ToString()),
        defaultSeverity: Info,
        isEnabledByDefault: true,
        description: "Reactive Extension methods that have scheduler overloads should explicitly specify a scheduler for better testability and predictable behavior.");

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

    public static DiagnosticDescriptor RSA1010 { get; } = new(
        id: "RSA1010",
        title: "Bind DynamicData changesets on the UI thread",
        messageFormat: "'{0}' should be preceded by ObserveOn to ensure the bound collection updates on the UI thread",
        CategoryMap.GetOrAdd(Usage, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "DynamicData's Bind operator writes changeset updates directly into a UI-bound collection; without an ObserveOn call earlier in the chain, those updates can arrive off the UI thread and corrupt the bound collection.");

    public static DiagnosticDescriptor RSA2001 { get; } = new(
        id: "RSA2001",
        title: "Constructors should appear before other members",
        messageFormat: "'{0}' ({1}) should appear before '{2}' ({3})",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "Constructors and destructors are declared first, whatever their accessibility, so the ways a type can be created lead the file.");

    public static DiagnosticDescriptor RSA2002 { get; } = new(
        id: "RSA2002",
        title: "Private members should appear after non-private members",
        messageFormat: "'{0}' ({1}) should appear before '{2}' ({3})",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "A type's public surface is declared before its implementation details, so a reader meets the contract before the machinery.");

    public static DiagnosticDescriptor RSA2003 { get; } = new(
        id: "RSA2003",
        title: "Members should be ordered by kind",
        messageFormat: "'{0}' ({1}) should appear before '{2}' ({3})",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "Within a group, members are declared in the order fields, events, properties, indexers, methods, operators, nested types. Within the private group specifically, fields sink below every other kind instead of leading it, since a private field is implementation detail backing the members above it.");

    public static DiagnosticDescriptor RSA2004 { get; } = new(
        id: "RSA2004",
        title: "Members should be ordered by access",
        messageFormat: "'{0}' ({1}) should appear before '{2}' ({3})",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "Within a kind, members are declared in the order public, internal, protected internal, protected, private protected, private.");

    public static DiagnosticDescriptor RSA2005 { get; } = new(
        id: "RSA2005",
        title: "Static members should appear before instance members",
        messageFormat: "'{0}' ({1}) should appear before '{2}' ({3})",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "Within a kind and accessibility, static members are declared before instance members.");

    public static DiagnosticDescriptor RSA2006 { get; } = new(
        id: "RSA2006",
        title: "Constant fields should appear before non-constant fields",
        messageFormat: "'{0}' ({1}) should appear before '{2}' ({3})",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "Constant fields are declared before other fields of the same accessibility.");

    public static DiagnosticDescriptor RSA2007 { get; } = new(
        id: "RSA2007",
        title: "Readonly fields should appear before mutable fields",
        messageFormat: "'{0}' ({1}) should appear before '{2}' ({3})",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "Readonly fields are declared before mutable fields of the same accessibility, so a reader sees what cannot change first.");

    public static DiagnosticDescriptor RSA2008 { get; } = new(
        id: "RSA2008",
        title: "File should contain a single type",
        messageFormat: "Move '{0}' to its own file",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "A file declares one top level type, so a type can be found from its file name alone.");

    public static DiagnosticDescriptor RSA2009 { get; } = new(
        id: "RSA2009",
        title: "File name should match the first type name",
        messageFormat: "File name '{0}' does not match the first type declared in it, '{1}'",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "A file is named for the type it declares, so a type can be found from its file name alone.");

    public static DiagnosticDescriptor RSA2010 { get; } = new(
        id: "RSA2010",
        title: "Do not use regions",
        messageFormat: "Remove the region directive",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "Regions hide code rather than organize it, and a region directive travels with the member below it.");

    public static DiagnosticDescriptor RSA2011 { get; } = new(
        id: "RSA2011",
        title: "Declare accessibility explicitly",
        messageFormat: "Declare an accessibility modifier for '{0}'",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "Accessibility is stated rather than inferred from the C# default, so the declared surface of a type is unambiguous.");

    public static DiagnosticDescriptor RSA2012 { get; } = new(
        id: "RSA2012",
        title: "Conditional compilation should not span member declarations",
        messageFormat: "Move the conditional compilation inside the members it guards, or split '{0}' across per-target files",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "A directive wrapping member declarations hides part of a type's surface and stops the reorder.");

    public static DiagnosticDescriptor RSA2013 { get; } = new(
        id: "RSA2013",
        title: "Line exceeds the maximum length",
        messageFormat: "Line is {0} characters long; the maximum is {1}",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "Lines stay inside the margin set by the max_line_length editorconfig key. The rule is silent where that key is absent or off.");

    public static DiagnosticDescriptor RSA2014 { get; } = new(
        id: "RSA2014",
        title: "Provide a summary for public abstractions",
        messageFormat: "'{0}' has no XML documentation summary",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "An interface, an abstract type, and the abstract or interface members they declare are a contract other code is written against, so each carries a <summary> a reader can act on without opening an implementation.");

    public static DiagnosticDescriptor RSA2015 { get; } = new(
        id: "RSA2015",
        title: "Provide <inheritdoc/> on members that implement or override an abstraction",
        messageFormat: "'{0}' implements or overrides a documented member; add <inheritdoc/>",
        CategoryMap.GetOrAdd(Design, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "A member that implements an interface member or overrides an abstract member restates a contract documented once at its source, so it points back there with <inheritdoc/> instead of duplicating or omitting the documentation.");

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