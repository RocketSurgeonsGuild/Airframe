using DynamicData;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ReactiveUI;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Splat;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Performance;

public class Rsa3002Tests
{
    [Theory]
    [InlineData(Rsa3002TestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3002>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA3002.Id));
    }

    [Theory]
    [InlineData(Rsa3002TestData.Incorrect)]
    [InlineData(Rsa3002TestData.ExtraIncorrect)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3002>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == RSA3002.Id));
    }

    [Theory]
    [InlineData(Rsa3002TestData.ExtraIncorrect)]
    public async Task GivenExtraIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3002>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa3002)]
           .Diagnostics
           .Should()
           .HaveCount(11)
           .And
           .Subject
           .Should()
           .OnlyContain(diagnostic => diagnostic.Id == RSA3002.Id);
    }

    [Theory]
    [InlineData(nameof(Rsa3002TestData.Correct), Rsa3002TestData.Correct)]
    [InlineData(nameof(Rsa3002TestData.Incorrect), Rsa3002TestData.Incorrect)]
    [InlineData(nameof(Rsa3002TestData.ExtraIncorrect), Rsa3002TestData.ExtraIncorrect)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3002>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(
                typeof(Unit),
                typeof(ICommand),
                typeof(ReactiveCommand),
                typeof(IObservableCache<int, string>),
                typeof(BindingList<int>),
                typeof(IEnableLogger),
                typeof(Expression<>),
                typeof(CompositeDisposable))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    [Theory]
    [InlineData(nameof(Rsa3002TestData.Incorrect), Rsa3002TestData.Incorrect)]
    public async Task GivenIncompatibleLanguageVersion_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3002>()
           .AddSources(source)
           .WithLanguageVersion(LanguageVersion.CSharp8)
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

    private static class Rsa3002TestData
    {
        internal const string Correct =

            // lang=csharp
            """
            using System;
            using System.Reactive;
            using System.Reactive.Disposables;
            using System.Reactive.Linq;
            using DynamicData;
            using ReactiveUI;

            namespace Sample
            {
                public class StaticLambdaExample : ReactiveObject
                {
                    public StaticLambdaExample()
                    {
                        Observable
                           .Return(Unit.Default)
                           .Select(static _ => _staticValue)
                           .Subscribe();
                
                        Observable
                           .Return(Unit.Default)
                           .Select(static _ => StaticValue)
                           .Subscribe();
                
                        Observable
                           .Return(Unit.Default)
                           .Select(static _ => GetStaticValue())
                           .Subscribe();
                
                        this.WhenAnyValue(static x => x.Life)
                           .Where(static x => x > 0)
                           .Subscribe(thing => SomeMethod(thing));
                
                        this.WhenAnyValue(static x => x.LifeChoices)
                           .Subscribe(static x => x.Choices.Question());
                
                        _sourceCache
                           .Connect()
                           .DeferUntilLoaded()
                           .RefCount()
                           .Filter(static life => life.Id > 0)
                           .AutoRefresh(static life => life.Choices)
                           .Bind(out var items)
                           .Subscribe();
                
                        Command = ReactiveCommand.Create(static () => { });
                    }
                
                    public int Life
                    {
                        get => _life;
                        set => this.RaiseAndSetIfChanged(ref _life, value);
                    }
                
                    public Life LifeChoices
                    {
                        get => _lifeChoices;
                        set => this.RaiseAndSetIfChanged(ref _lifeChoices, value);
                    }
                
                    public ReactiveCommand<Unit, Unit> Command { get; }
                
                    public void SomeMethod(int thing)
                    {
                    }
                
                    public static int StaticValue { get; } = 100;
                
                    private static int GetStaticValue() => 200;
                
                    private static readonly int _staticValue = 42;
                
                    private int _life;
                    private SourceCache<Life, int> _sourceCache = new SourceCache<Life, int>(static x => x.Id);
                    private Life _lifeChoices = null!;
                }
                
                public class Life : ReactiveObject
                {
                    public int Id { get; set; }
                
                    public Choices Choices
                    {
                        get => _choices;
                        set => this.RaiseAndSetIfChanged(ref _choices, value);
                    }
                
                    private Choices _choices;
                }
                
                public class Choices : ReactiveObject
                {
                    public void Question() { }
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
                public class StaticLambdaExample : ReactiveObject
                {
                    public StaticLambdaExample()
                    {
                        Observable
                            .Return(Unit.Default)
                            .Select(_ => _staticValue)
                            .Subscribe();

                        Observable
                            .Return(Unit.Default)
                            .Select(_ => StaticValue)
                            .Subscribe();

                        Observable
                            .Return(Unit.Default)
                            .Select(_ => GetStaticValue())
                            .Subscribe();
                            
                        Command = ReactiveCommand.Create(() => { });
                    }
                
                    public ReactiveCommand<Unit, Unit> Command { get; }

                    public static int StaticValue { get; } = 100;
                    
                    private static int GetStaticValue() => 200;

                    private static readonly int _staticValue = 42;
                }
            }
            """;

        internal const string ExtraIncorrect =

            // lang=csharp
            """
            using DynamicData;
            using System;
            using System.Reactive;
            using System.Reactive.Disposables;
            using System.Reactive.Linq;
            using ReactiveUI;

            namespace Sample
            {
                public class StaticLambdaExample : ReactiveObject
                {
                    public StaticLambdaExample()
                    {
                        Observable
                           .Return(Unit.Default)
                           .Select(_ => _staticValue)
                           .Subscribe();

                        Observable
                           .Return(Unit.Default)
                           .Select(_ => StaticValue)
                           .Subscribe();

                        Observable
                           .Return(Unit.Default)
                           .Select(_ => GetStaticValue())
                           .Subscribe();

                        this.WhenAnyValue(x => x.Life)
                           .Where(x => x > 0)
                           .Subscribe(thing => SomeMethod(thing));

                        this.WhenAnyValue(x => x.LifeChoices)
                           .Subscribe(x => x.Choices.Question());

                        _sourceCache
                           .Connect()
                           .DeferUntilLoaded()
                           .RefCount()
                           .Filter(life => life.Id > 0)
                           .AutoRefresh(life => life.Choices)
                           .Bind(out var items)
                           .Subscribe();

                        Command = ReactiveCommand.Create(() => { });
                    }

                    public int Life
                    {
                        get => _life;
                        set => this.RaiseAndSetIfChanged(ref _life, value);
                    }

                    public Life LifeChoices
                    {
                        get => _lifeChoices;
                        set => this.RaiseAndSetIfChanged(ref _lifeChoices, value);
                    }

                    public ReactiveCommand<Unit, Unit> Command { get; }

                    public void SomeMethod(int thing) { }

                    public static int StaticValue { get; } = 100;

                    private static int GetStaticValue() => 200;
                    private static readonly int _staticValue = 42;

                    private int _life;
                    private SourceCache<Life, int> _sourceCache = new SourceCache<Life, int>(x => x.Id);
                    private Life _lifeChoices;
                }

                public class Life : ReactiveObject
                {
                    public int Id { get; set; }

                    public Choices Choices
                    {
                        get => _choices;
                        set => this.RaiseAndSetIfChanged(ref _choices, value);
                    }

                    private Choices _choices;
                }

                public class Choices : ReactiveObject
                {
                    public void Question() { }
                }
            }
            """;
    }
}