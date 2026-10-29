using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis.Diagnostics;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Nullability;

/// <summary>
/// RSA0002, RSA0003, and RSA0006 see a positional record parameter's synthesized property.
/// </summary>
public class PositionalRecordTests
{
    [Fact]
    public async Task GivenNullableCollectionParameter_WhenAnalyze_ThenRsa0002Reported() =>
        await ReportsOnce<Rsa0002>(RSA0002.Id, "public record Order(string[]? Items);");

    [Fact]
    public async Task GivenNullableTaskParameter_WhenAnalyze_ThenRsa0003Reported() =>
        await ReportsOnce<Rsa0003>(RSA0003.Id, "public record Work(System.Threading.Tasks.Task? Run);");

    [Fact]
    public async Task GivenNullableBooleanParameter_WhenAnalyze_ThenRsa0006Reported() =>
        await ReportsOnce<Rsa0006>(RSA0006.Id, "public record T(bool? A);");

    [Fact]
    public async Task GivenRecordStructParameter_WhenAnalyze_ThenRsa0006Reported() =>
        await ReportsOnce<Rsa0006>(RSA0006.Id, "public record struct T(bool? A);");

    [Fact]
    public async Task GivenParameterAndDeclaredProperty_WhenAnalyze_ThenEachReportedOnce() =>
        await ReportsOnce<Rsa0006>(RSA0006.Id, "public record T(bool? A) { public bool? B { get; init; } }", expected: 2);

    [Fact]
    public async Task GivenParameterWithExplicitProperty_WhenAnalyze_ThenReportedOnceOnDeclaredProperty() =>
        await ReportsOnce<Rsa0006>(RSA0006.Id, "public record T(bool? A) { public bool? A { get; init; } }");

    [Theory]
    [InlineData("public record T(bool A);")]
    [InlineData("public record T(int? A);")]
    [InlineData("internal record T(bool? A);")]
    [InlineData("public class Outer { private record T(bool? A); }")]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string declaration)
    {
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources($"namespace Sample {{ {declaration} }}")
           .WithAnalyzer<Rsa0006>()
           .GenerateAsync();

        result.AnalyzerResults[typeof(Rsa0006)].Diagnostics.Should().NotContain(d => d.Id == RSA0006.Id);
    }

    private static async Task ReportsOnce<TAnalyzer>(string id, string declaration, int expected = 1)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources($"namespace Sample {{ {declaration} }}")
           .WithAnalyzer<TAnalyzer>()
           .GenerateAsync();

        result.AnalyzerResults[typeof(TAnalyzer)].Diagnostics.Count(d => d.Id == id).Should().Be(expected);
    }
}
