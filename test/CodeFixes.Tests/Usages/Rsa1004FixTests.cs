using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;
using Rocket.Surgery.Airframe.CodeFixes.Usage;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Usages;

public class Rsa1004FixTests
{
    [Theory]
    [InlineData(Rsa1004FixTestData.IncorrectSingleProperty, 1)]
    [InlineData(Rsa1004FixTestData.IncorrectMultipleProperty, 4)]
    public async Task GivenSubscription_WhenVerified_ThenDiagnosticsResolved(string incorrect, int diagnosticCount)
    {
        // Given, When, Then
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1004>()
           .WithCodeFix<Rsa1004Fix>()
           .AddSources(incorrect)
           .Build()
           .GenerateAsync();

        result
           .CodeFixResults[typeof(Rsa1004Fix)]
           .ResolvedFixes
           .Should()
           .HaveCount(diagnosticCount)
           .And
           .Subject
           .Should()
           .Contain(testResult => testResult.Diagnostic.Descriptor == Descriptions.RSA1004);
    }

    [Theory(Skip = "Verify output seems odd.")]
    [InlineData(nameof(Rsa1004FixTestData.Incorrect), Rsa1004FixTestData.Incorrect)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1004>()
           .WithCodeFix<Rsa1004Fix>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(
                typeof(Unit),
                typeof(ICommand),
                typeof(ReactiveCommand),
                typeof(BindingList<int>),
                typeof(Expression<>),
                typeof(CompositeDisposable))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }
}

internal static class Rsa1004FixTestData
{
    internal const string Incorrect = @"
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace Sample
{
    public class OutParameterAssignmentExample : ReactiveObject
    {
        public OutParameterAssignmentExample()
        {
            var _ =
                Observable
                   .Return(Unit.Default)
                   .ToProperty(this, x => x.Value);
        }

        public Unit Value => _value.Value;

        private readonly ObservableAsPropertyHelper<Unit> _value = ObservableAsPropertyHelper<Unit>.Default();
    }
}";
    internal const string IncorrectSingleProperty = @"
using System;
using System.Reactive;
using ReactiveUI;

namespace Sample
{
    public class WhenAnyValueClosureExample : ReactiveObject
    {
        public WhenAnyValueClosureExample() => this.WhenAnyValue(x => Value).Subscribe();

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}";
    internal const string IncorrectMultipleProperty = @"
using System;
using System.Reactive;
using ReactiveUI;

namespace Sample
{
    public class WhenAnyValueClosureExample : ReactiveObject
    {
        public WhenAnyValueClosureExample()
        {
            this.WhenAnyValue(x => Value, y => y.Value).Subscribe();
            this.WhenAnyValue(x => x.Value, y => Value).Subscribe();
            this.WhenAnyValue(x => Value, y => Value).Subscribe();
        }

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}";
}