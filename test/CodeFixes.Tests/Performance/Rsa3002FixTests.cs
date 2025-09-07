using DynamicData;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;
using Rocket.Surgery.Airframe.CodeFixes.Performance;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Splat;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Performance;

public class Rsa3002FixTests
{
    [Theory]
    [ClassData(typeof(Rsa3002FixTestData))]
    public async Task GivenSubscription_WhenVerified_ThenDiagnosticsResolved(string incorrect, int diagnosticCount)
    {
        // Given, When, Then
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3002>()
           .WithCodeFix<Rsa3002Fix>()
           .AddSources(incorrect)
           .Build()
           .GenerateAsync();

        result
           .CodeFixResults[typeof(Rsa3002Fix)]
           .ResolvedFixes
           .Should()
           .HaveCount(diagnosticCount)
           .And
           .Subject
           .Should()
           .Contain(testResult => testResult.Diagnostic.Descriptor == Descriptions.RSA3002);
    }

    [Theory(Skip = "Verify output seems odd.")]
    [InlineData(nameof(Rsa3002FixTestData.Incorrect), Rsa3002FixTestData.Incorrect)]
    [InlineData(nameof(Rsa3002FixTestData.ExtraIncorrect), Rsa3002FixTestData.ExtraIncorrect)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3002>()
           .WithCodeFix<Rsa3002Fix>()
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
}

public class Rsa3002FixTestData : IEnumerable<object[]>
{
    /// <inheritdoc/>
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            Incorrect,
            4
        ];
        yield return
        [
            ExtraIncorrect,
            11
        ];
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

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
                // Static field
                private static readonly int _staticValue = 42;
                
                public StaticLambdaExample()
                {
                    // Using static field - should warn
                    Observable
                        .Return(Unit.Default)
                        .Select(_ => _staticValue)
                        .Subscribe();

                    // Using static property - should warn
                    Observable
                        .Return(Unit.Default)
                        .Select(_ => StaticValue)
                        .Subscribe();
                        
                    // Using static method - should warn
                    Observable
                        .Return(Unit.Default)
                        .Select(_ => GetStaticValue())
                        .Subscribe();
                        
                    Command = ReactiveCommand.Create(() => { });
                }

                public ReactiveCommand<Unit, Unit> Command { get; }
                
                // Static property
                public static int StaticValue { get; } = 100;
                
                // Static method
                private static int GetStaticValue() => 200;
            }
        }
        """;

    internal const string ExtraIncorrect =

        // lang=csharp
        """
        using DynamicData;
        using ReactiveUI;
        using System;
        using System.Reactive;
        using System.Reactive.Linq;

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
                       .WhereReasonsAre(ChangeReason.Refresh, ChangeReason.Update)
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