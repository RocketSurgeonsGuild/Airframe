using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics;

public class Rsa1006Tests
{
    [Theory]
    [InlineData(Rsa1006TestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1006>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA1006.Id));
    }

    [Theory]
    [InlineData(Rsa1006TestData.Incorrect)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1006>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == RSA1006.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa1006TestData.Correct), Rsa1006TestData.Correct)]
    [InlineData(nameof(Rsa1006TestData.Incorrect), Rsa1006TestData.Incorrect)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1006>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(
                typeof(Unit),
                typeof(ICommand),
                typeof(ReactiveCommand),
                typeof(Expression<>),
                typeof(IServiceProvider),
                typeof(SynchronizationContext))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa1006TestData
    {
        internal const string Correct = @"
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;

namespace Sample
{
    public class MultipleUsesOfSubscribeOnExample : ReactiveObject
    {

        public MultipleUsesOfSubscribeOnExample() => Observable
                                                     .Return(Unit.Default)
                                                     .SubscribeOn(TaskPoolScheduler.Default);
    }
}";

        internal const string Incorrect = @"
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;

namespace Sample
{
    public class MultipleUsesOfSubscribeOnExample : ReactiveObject
    {

        public MultipleUsesOfSubscribeOnExample() => Observable
                                                     .Return(Unit.Default)
                                                     .SubscribeOn(TaskPoolScheduler.Default)
                                                     .SubscribeOn(ImmediateScheduler.Instance);
    }
}";
    }
}