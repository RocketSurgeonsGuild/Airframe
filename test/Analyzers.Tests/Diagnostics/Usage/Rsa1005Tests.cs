using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Usage;

public class Rsa1005Tests
{
    [Theory]
    [InlineData(Rsa1005TestData.Incorrect)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1005>()
           .AddReferences(typeof(Unit), typeof(Expression<>))
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == Descriptions.RSA1005.Id) && pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == Descriptions.RSA1005.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa1005TestData.Incorrect), Rsa1005TestData.Incorrect)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1005>()
           .AddSources(source)
           .AddReferences(typeof(Unit), typeof(Expression<>))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa1005TestData
    {
        // lang=csharp
        public const string Incorrect = """
            using System;
            using System.Reactive;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;

            namespace Foo.Bar;

            public class Rsa1005Example
            {
                public Rsa1005Example() =>
                    Observable
                       .Return(Unit.Default)
                       .Throttle(TimeSpan.Zero)
                       .Subscribe();
            }
            """;
    }
}