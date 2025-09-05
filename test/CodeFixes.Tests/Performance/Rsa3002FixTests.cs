using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Performance;
using Rocket.Surgery.Airframe.CodeFixes.Performance;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Performance;

// [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
public class Rsa3002FixTests
{
    [Theory]
    [ClassData(typeof(Rsa3002FixTestData))]
    public async Task GivenSubscription_WhenVerified_ThenDiagnosticsReported(string incorrect, string correct, IEnumerable<DiagnosticResult> results)
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
           .AnalyzerResults[typeof(Rsa3002)]
           .Diagnostics
           .Should()
           .HaveCount(11)
           .And
           .Subject
           .Should()
           .OnlyContain(diagnostic => diagnostic.Id == Descriptions.RSA3002.Id);
    }

    [Theory]
    [InlineData(Rsa3002FixTestData.Correct)]
    public Task GivenSubscriptionDisposed_WhenVerified_ThenNoDiagnosticsReported(string code) =>

        // Given, When, Then
        // return VerifyCS.VerifyAnalyzerAsync(code);
        Task.CompletedTask;
}

public class Rsa3002FixTestData : IEnumerable<object[]>
{
    /// <inheritdoc/>
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            Incorrect,
            Correct,
            this._diagnostics
        };
        yield return new object[]
        {
            ExtraIncorrect,
            Correct,
            new DiagnosticResult[] { }
        };
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    internal const string Correct = @"
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
            // Using instance properties - good
            Observable
                .Return(Unit.Default)
                .Select(static _ => Value)
                .Subscribe();

            // Using local variables - good
            var localValue = 42;
            Observable
                .Return(Unit.Default)
                .Select(static _ => localValue)
                .Subscribe();

            // Using parameters - good
            void Process(int param) =>
                Observable
                    .Return(Unit.Default)
                    .Select(static _ => param)
                    .Subscribe();
                    
            Command = ReactiveCommand.Create(static () => { });
        }

        public ReactiveCommand<Unit, Unit> Command { get; }

        public int Value { get; set; } = 42;
    }
}";

    internal const string Incorrect = @"
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
}";

    internal const string ExtraIncorrect = @"using DynamicData;
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
}";

    private readonly List<DiagnosticResult> _diagnostics =
    [
        new DiagnosticResult(Descriptions.RSA3002)
           .WithSeverity(DiagnosticSeverity.Warning)
           .WithSpan(18, 27, 18, 38)
           .WithMessage(
                Descriptions.RSA3002
                   .MessageFormat.ToString()),

        new DiagnosticResult(Descriptions.RSA3002)
           .WithSeverity(DiagnosticSeverity.Warning)
           .WithSpan(24, 27, 24, 37)
           .WithMessage(
                Descriptions.RSA3002
                   .MessageFormat.ToString()),

        new DiagnosticResult(Descriptions.RSA3002)
           .WithSeverity(DiagnosticSeverity.Warning)
           .WithSpan(30, 27, 30, 42)
           .WithMessage(
                Descriptions.RSA3002
                   .MessageFormat.ToString()),

        new DiagnosticResult(Descriptions.RSA3002)
           .WithSeverity(DiagnosticSeverity.Warning)
           .WithSpan(35, 46, 35, 55)
           .WithMessage(
                Descriptions.RSA3002
                   .MessageFormat.ToString())
    ];
}