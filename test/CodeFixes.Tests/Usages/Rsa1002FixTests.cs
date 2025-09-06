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

public class Rsa1002FixTests
{
    [Theory]
    [InlineData(Rsa1002FixTestData.Incorrect, 1)]
    public async Task GivenSubscription_WhenVerified_ThenDiagnosticsResolved(string incorrect, int diagnosticCount)
    {
        // Given, When, Then
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1002>()
           .WithCodeFix<Rsa1002Fix>()
           .AddSources(incorrect)
           .Build()
           .GenerateAsync();

        result
           .CodeFixResults[typeof(Rsa1002Fix)]
           .ResolvedFixes
           .Should()
           .HaveCount(diagnosticCount)
           .And
           .Subject
           .Should()
           .Contain(testResult => testResult.Diagnostic.Descriptor == Descriptions.RSA1002);
    }

    [Theory(Skip = "Verify output seems odd.")]
    [InlineData(nameof(Rsa1002FixTestData.Incorrect), Rsa1002FixTestData.Incorrect)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1002>()
           .WithCodeFix<Rsa1002Fix>()
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

public static class Rsa1002FixTestData
{
    internal const string Incorrect =
        """
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
        }
        """;
}