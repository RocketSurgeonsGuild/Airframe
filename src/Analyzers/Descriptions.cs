using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;
using static Rocket.Surgery.Airframe.Analyzers.Category;

namespace Rocket.Surgery.Airframe.Analyzers;

internal static class Descriptions
{
    private static readonly ConcurrentDictionary<Category, string> CategoryMap = new();

    public static DiagnosticDescriptor RSA0002 { get; } = new(
        id: "RSA0002",
        title: "Do not return a nullable collection",
        messageFormat: "'{0}' returns a nullable collection; return an empty collection instead of null",
        CategoryMap.GetOrAdd(Nullability, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "A public or protected member that returns a nullable collection type forces every caller across that boundary to null-check before it can enumerate. Return an empty collection - Array.Empty<T>(), Enumerable.Empty<T>(), or a collection expression - instead of null.");

    public static DiagnosticDescriptor RSA0003 { get; } = new(
        id: "RSA0003",
        title: "Do not return a nullable task",
        messageFormat: "'{0}' returns a nullable {1}; a task represents a completion and should never itself be null",
        CategoryMap.GetOrAdd(Nullability, category => category.ToString()),
        defaultSeverity: Warning,
        isEnabledByDefault: true,
        description: "A public or protected member that declares a nullable Task, Task<T>, ValueTask, or ValueTask<T> return type forces every caller to null-check the task itself before awaiting it. Return Task.CompletedTask or Task.FromResult instead. This does not apply to Task<T?>, where only the completed result is nullable.");

    public static DiagnosticDescriptor RSA0004 { get; } = new(
        id: "RSA0004",
        title: "Null-forgiving operator used too many times in one member",
        messageFormat: "'{0}' uses the null-forgiving operator (!) {1} times; more than {2} in a single member suggests a suppressed design defect rather than a fix",
        CategoryMap.GetOrAdd(Nullability, category => category.ToString()),
        defaultSeverity: Info,
        isEnabledByDefault: true,
        description: "The null-forgiving operator overrides the compiler's nullable flow analysis one expression at a time, so no single use is ever wrong on its own; density is the signal. A member relying on it more than the configured threshold - default 3, configurable with the rsa0004_max_null_forgiving_operators editorconfig key - is suppressing a design defect rather than fixing it. There is no code fix: there is nothing mechanical to apply, since the fix is a design change only the author can make.");

    public static DiagnosticDescriptor RSA0006 { get; } = new(
        id: "RSA0006",
        title: "Do not use a nullable boolean to model three states",
        messageFormat: "'{0}' is a nullable bool; a named enum expresses the three states more clearly than true, false, and null",
        CategoryMap.GetOrAdd(Nullability, category => category.ToString()),
        defaultSeverity: Info,
        isEnabledByDefault: true,
        description: "A public or protected bool? property is often modeling three states, not two, with true, false, and null each carrying a distinct meaning. A named enum expresses those states explicitly instead of relying on the reader to know what null means here. An override and an explicit interface implementation are excluded, since neither author chose that member's type - the enum change belongs at the origin of the contract instead.");

    public static DiagnosticDescriptor RSA0009 { get; } = new(
        id: "RSA0009",
        title: "Do not declare a nullable reference return type on an abstraction member",
        messageFormat: "'{0}' is abstract, virtual, or an interface member; its nullable reference return type is forced onto every override and implementation",
        CategoryMap.GetOrAdd(Nullability, category => category.ToString()),
        defaultSeverity: Info,
        isEnabledByDefault: false,
        description: "An abstract, virtual, or interface member that declares a nullable reference return type forces that nullability onto every override and every implementation, irreversibly - none of them chose it. Disabled by default: locating a layer boundary where a nullable abstraction is genuinely the wrong call requires project-specific configuration this analyzer does not have. A collection-shaped or task-shaped return type is excluded here, since RSA0002 and RSA0003 already report those at Warning severity; this rule covers everything else.");

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
        description: "Lines stay inside the margin set by the max_line_length editorconfig key, measured by code content alone — a trailing comment or an XML doc/comment line does not count. The rule is silent where that key is absent or off.");

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

    public static DiagnosticDescriptor RSA3003 { get; } =
        new(
            "RSA3003",
            "Lambda expression captures state and allocates a closure",
            "Lambda expression captures state and allocates a closure",
            CategoryMap.GetOrAdd(Performance, category => category.ToString()),
            Info,
            true,
            "Lambda expressions that capture a local variable or instance state, including implicit access to 'this', allocate a closure on every invocation.");

    public static DiagnosticDescriptor RSA3004 { get; } =
        new(
            "RSA3004",
            "Provide an explicit scheduler for AutoRefresh",
            "AutoRefresh re-evaluates on every change; provide an explicit IScheduler to control where that work runs (e.g. off the UI thread on a task pool)",
            CategoryMap.GetOrAdd(Performance, category => category.ToString()),
            Warning,
            true,
            "AutoRefresh re-evaluates its selector on every property change notification. Without an explicit scheduler that work runs wherever the source notifications originate, which is often the UI thread.");

    public static DiagnosticDescriptor RSA3005 { get; } =
        new(
            "RSA3005",
            "AutoRefresh already applied for this property",
            "AutoRefresh is already applied for this property earlier in the chain",
            CategoryMap.GetOrAdd(Performance, category => category.ToString()),
            Warning,
            true,
            "Applying AutoRefresh more than once for the same property within the same observable chain re-evaluates that property redundantly.");
}