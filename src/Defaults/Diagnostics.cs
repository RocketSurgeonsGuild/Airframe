// <copyright file="Diagnostics.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults;

public static class Diagnostics
{
    /// <summary>
    ///     Diagnostic for unsupported classes with no constructors.
    /// </summary>
    public static DiagnosticDescriptor Defaults0001 { get; } = new(
        nameof(Defaults0001),
        "Inaccessible constructor.",
        string.Empty,
        "Support",
        DiagnosticSeverity.Error,
        true);
}