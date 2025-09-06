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

public class Rsa3001FixTests
{
    [Theory]
    [ClassData(typeof(Rsa3001FixTestData))]
    public async Task GivenSubscription_WhenVerified_ThenDiagnosticsResolved(string incorrect, int diagnosticCount)
    {
        // Given, When, Then
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3001>()
           .WithCodeFix<Rsa3001Fix>()
           .AddSources(incorrect)
           .Build()
           .GenerateAsync();

        result
           .CodeFixResults[typeof(Rsa3001Fix)]
           .ResolvedFixes
           .Should()
           .HaveCount(diagnosticCount)
           .And
           .Subject
           .Should()
           .Contain(testResult => testResult.Diagnostic.Descriptor == Descriptions.RSA3001);
    }

    [Theory(Skip = "Verify output seems odd.")]
    [InlineData(nameof(Rsa3001FixTestData.Incorrect), Rsa3001FixTestData.Incorrect)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3001>()
           .WithCodeFix<Rsa3001Fix>()
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

public class Rsa3001FixTestData : IEnumerable<object[]>
{
    /// <inheritdoc/>
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            Incorrect,
            4
        ];
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal const string Correct = @"
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
}";

    internal const string Incorrect = @"
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
}";
}