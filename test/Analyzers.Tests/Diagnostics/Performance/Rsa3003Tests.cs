using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;
using VerifyXunit;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Performance;

public class Rsa3003Tests
{
    [Theory]
    [InlineData(Rsa3003TestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3003>()
           .AddReferences(typeof(Unit), typeof(Expression<>))
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA3003.Id));
    }

    [Theory]
    [InlineData(Rsa3003TestData.Incorrect)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3003>()
           .AddReferences(typeof(Unit), typeof(Expression<>))
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA3003.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa3003TestData.Correct), Rsa3003TestData.Correct)]
    [InlineData(nameof(Rsa3003TestData.Incorrect), Rsa3003TestData.Incorrect)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3003>()
           .AddSources(source)
           .AddReferences(typeof(Unit), typeof(Expression<>))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa3003TestData
    {
        // lang=csharp
        internal const string Correct = """
            using System;
            using System.Reactive;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Rsa3003Example
                {
                    public Rsa3003Example()
                    {
                        Observable
                           .Return(Unit.Default)
                           .Select(_ => GetStaticValue())
                           .Subscribe();
                    }

                    private static int GetStaticValue() => 200;
                }
            }
            """;

        // lang=csharp
        internal const string Incorrect = """
            using System;
            using System.Reactive;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Rsa3003Example
                {
                    private readonly OtherState _otherState = new OtherState();

                    public Rsa3003Example()
                    {
                        Observable
                           .Return(Unit.Default)
                           .Where(_ => _otherState.State)
                           .Subscribe();
                    }
                }

                public class OtherState
                {
                    public bool State { get; set; }
                }
            }
            """;
    }
}
