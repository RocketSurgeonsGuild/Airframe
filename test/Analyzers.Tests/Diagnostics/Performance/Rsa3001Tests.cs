using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Splat;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Performance;

public class Rsa3001Tests
{
    [Theory]
    [InlineData(Rsa3001TestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3001>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA3001.Id));
    }

    [Theory]
    [InlineData(Rsa3001TestData.Incorrect)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3001>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == RSA3001.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa3001TestData.Correct), Rsa3001TestData.Correct)]
    [InlineData(nameof(Rsa3001TestData.Incorrect), Rsa3001TestData.Incorrect)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3001>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(
                typeof(Unit),
                typeof(ICommand),
                typeof(ReactiveCommand),
                typeof(IEnableLogger),
                typeof(Expression<>),
                typeof(CompositeDisposable))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa3001TestData
    {
        internal const string Correct =

            // lang=csharp
            """
            using System;
            using System.Reactive;
            using System.Reactive.Disposables;
            using System.Reactive.Linq;
            using ReactiveUI;

            namespace Sample
            {
                public class SubscriptionDisposalExample : ReactiveObject
                {
                    public SubscriptionDisposalExample()
                    {
                        Observable
                           .Return(Unit.Default)
                           .BindTo(this, x => x.Unit)
                           .DisposeWith(Garbage);

                        Observable
                           .Return(Unit.Default)
                           .InvokeCommand(this, x => x.Command)
                           .DisposeWith(Garbage);

                        Observable
                           .Return(Unit.Default)
                           .Subscribe()
                           .DisposeWith(Garbage);

                        Observable
                           .Return(Unit.Default)
                           .ToProperty(this, nameof(Value), out _value)
                           .DisposeWith(Garbage);

                        Command = ReactiveCommand.Create(() => { });
                    }

                    public ReactiveCommand<Unit, Unit> Command { get; }

                    public Unit Value => _value.Value;
                
                    public Unit Unit
                    {
                        get => _unit;
                        set => this.RaiseAndSetIfChanged(ref _unit, value);
                    }

                    private readonly CompositeDisposable Garbage = new CompositeDisposable();

                    private readonly ObservableAsPropertyHelper<Unit> _value;
                    private Unit _unit;
                }
            }
            """;

        internal const string Incorrect =

            // lang=csharp
            """
            using System;
            using System.Reactive;
            using System.Reactive.Disposables;
            using System.Reactive.Linq;
            using ReactiveUI;

            namespace Sample
            {
                public class SubscriptionDisposalExample : ReactiveObject
                {
                    public SubscriptionDisposalExample()
                    {
                        Observable
                           .Return(Unit.Default)
                           .BindTo(this, x => x.Unit);
                
                        Observable
                           .Return(Unit.Default)
                           .InvokeCommand(this, x => x.Command);
                
                        Observable
                           .Return(Unit.Default)
                           .Subscribe();
                
                        Observable
                           .Return(Unit.Default)
                           .ToProperty(this, nameof(Value), out _value);
                
                        Command = ReactiveCommand.Create(() => { });
                    }
                
                    public ReactiveCommand<Unit, Unit> Command { get; }
                
                    public Unit Value => _value.Value;
                
                    public Unit Unit
                    {
                        get => _unit;
                        set => this.RaiseAndSetIfChanged(ref _unit, value);
                    }
                
                    private readonly CompositeDisposable Garbage = new CompositeDisposable();
                
                    private readonly ObservableAsPropertyHelper<Unit> _value;
                    private Unit _unit;
                }
            }
            """;
    }
}