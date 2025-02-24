using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults;

internal static class Diagnostics
{
    /// <summary>
    ///    Gets a diagnostic for unsupported classes with no constructors.
    /// </summary>
    public static DiagnosticDescriptor Rsad0001 { get; } = new(
        nameof(Rsad0001).ToUpperInvariant(),
        "Inaccessible constructor.",
        string.Empty,
        "Support",
        DiagnosticSeverity.Error,
        true);
}