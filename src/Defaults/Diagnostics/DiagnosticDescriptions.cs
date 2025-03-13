using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults.Diagnostics;

internal static class DiagnosticDescriptions
{
    /// <summary>
    ///    Gets a diagnostic for unsupported classes with no constructors.
    /// </summary>
    public static DiagnosticDescriptor Rsad0001 { get; } = new(
        nameof(Rsad0001).ToUpperInvariant(),
        "Inaccessible constructor",
        string.Empty,
        "Support",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    ///    Gets a diagnostic for unsupported classes with multiple constructors.
    /// </summary>
    public static DiagnosticDescriptor Rsad0002 { get; } = new(
        nameof(Rsad0002).ToUpperInvariant(),
        "Multiple constructors",
        string.Empty,
        "Support",
        DiagnosticSeverity.Error,
        true);
}