using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Splat;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics;

public class Rsa1002Tests
{
    [Theory]
    [InlineData(Rsa1002TestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1002>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == Descriptions.RSA1002.Id));
    }

    [Theory]
    [InlineData(Rsa1002TestData.Incorrect)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1002>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == Descriptions.RSA1002.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa1002TestData.Correct), Rsa1002TestData.Correct)]
    [InlineData(nameof(Rsa1002TestData.Incorrect), Rsa1002TestData.Incorrect)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1002>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(typeof(Unit), typeof(ICommand), typeof(ReactiveCommand), typeof(Expression<>), typeof(IEnableLogger))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa1002TestData
    {
        internal const string Correct = @"
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace Sample
{
    public class BindToClosureExample : ReactiveObject
    {
        public BindToClosureExample() => Observable
           .Return(Unit.Default)
           .BindTo(this, x => x.Value);

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}";

        internal const string Incorrect = @"
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace Sample
{
    public class BindToClosureExample : ReactiveObject
    {
        public BindToClosureExample() => Observable
           .Return(Unit.Default)
           .BindTo(this, x => Value);

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}";
    }
}