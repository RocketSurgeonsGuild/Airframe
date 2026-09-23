using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;
using System;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.Airframe.Analyzers.Tests;

/// <summary>
/// Regression tests for analyzer discovery (#360). Verifies that
/// <see cref="DiagnosticAnalyzerAttribute"/> (with Inherited=false) is correctly applied to
/// concrete analyzers and absent from abstract bases.
/// </summary>
public class AnalyzerDiscoveryTests
{
    [Fact]
    public void GivenConcreteAnalyzers_WhenInspected_ThenAllHaveDiagnosticAnalyzerAttribute()
    {
        // Given
        var analyzerAssembly = typeof(Rsa1000).Assembly;
        var concreteAnalyzerTypes = analyzerAssembly
           .GetTypes()
           .Where(t => typeof(DiagnosticAnalyzer).IsAssignableFrom(t) && !t.IsAbstract)
           .ToList();

        // When
        concreteAnalyzerTypes.Should().NotBeEmpty("at least one concrete analyzer should exist");

        // Then
        concreteAnalyzerTypes
           .Should()
           .OnlyContain(
                t => t.GetCustomAttributes<DiagnosticAnalyzerAttribute>().Any(),
                because: "every concrete DiagnosticAnalyzer subclass must have [DiagnosticAnalyzer] applied (Inherited=false prevents inheritance)"
            );
    }

    [Fact]
    public void GivenAbstractAnalyzerBases_WhenInspected_ThenNoneHaveDiagnosticAnalyzerAttribute()
    {
        // Given
        var abstractAnalyzerTypes = new[] { typeof(Rsa1000), typeof(Rsa3000) };

        // When
        abstractAnalyzerTypes.Should().OnlyContain(t => t.IsAbstract, "both bases should remain abstract");

        // Then
        abstractAnalyzerTypes
           .Should()
           .AllSatisfy(
                t => t.GetCustomAttributes<DiagnosticAnalyzerAttribute>().Should().BeEmpty(),
                because: "abstract base classes should not carry [DiagnosticAnalyzer]; only concrete implementations need it"
            );
    }
}
