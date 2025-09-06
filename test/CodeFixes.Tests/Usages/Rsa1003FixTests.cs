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

public class Rsa1003FixTests
{
    [Theory]
    [InlineData(Rsa1003FixTestData.Incorrect, 1)]
    public async Task GivenSubscription_WhenVerified_ThenDiagnosticsResolved(string incorrect, int diagnosticCount)
    {
        // Given, When, Then
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1003>()
           .WithCodeFix<Rsa1003Fix>()
           .AddSources(incorrect)
           .Build()
           .GenerateAsync();

        result
           .CodeFixResults[typeof(Rsa1003Fix)]
           .ResolvedFixes
           .Should()
           .HaveCount(diagnosticCount)
           .And
           .Subject
           .Should()
           .Contain(testResult => testResult.Diagnostic.Descriptor == Descriptions.RSA1003);
    }

    [Theory(Skip = "Verify output seems odd.")]
    [InlineData(nameof(Rsa1003FixTestData.Incorrect), Rsa1003FixTestData.Incorrect)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1003>()
           .WithCodeFix<Rsa1003Fix>()
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

internal static class Rsa1003FixTestData
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
}