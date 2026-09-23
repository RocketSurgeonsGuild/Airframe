using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
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

public class Rsa3005Tests
{
    [Theory]
    [InlineData(Rsa3005TestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3005>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA3005.Id));
    }

    [Theory]
    [InlineData(Rsa3005TestData.Incorrect)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3005>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == RSA3005.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa3005TestData.Correct), Rsa3005TestData.Correct)]
    [InlineData(nameof(Rsa3005TestData.Incorrect), Rsa3005TestData.Incorrect)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3005>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(
                typeof(Unit),
                typeof(ICommand),
                typeof(ReactiveCommand),
                typeof(IActivatableView),
                typeof(IEnableLogger),
                typeof(Expression<>),
                typeof(CompositeDisposable))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    /// <summary>
    /// RSA3001 ("subscription not disposed") and RSA3005 ("wrong disposable target") are not
    /// mutually exclusive triggers on the same line, but they must never double-report: RSA3001
    /// only fires on a bare, undisposed subscription statement, and a DisposeWith call - correct
    /// or wrong-target - is never a bare statement, so RSA3001 stays silent whenever RSA3005 has
    /// something to inspect. These two tests confirm that empirically, running both analyzers
    /// together over the same sources used above. This one uses only the correctly-disposed
    /// snippet, not the full <see cref="Rsa3005TestData.Correct"/> source - that source also
    /// contains a deliberately bare, undisposed subscription whose whole point is to demonstrate
    /// that RSA3001 (not RSA3005) is responsible for flagging it.
    /// </summary>
    [Fact]
    public async Task GivenCorrectlyScopedDisposal_WhenAnalyzedWithRsa3001AndRsa3005_ThenNeitherFires()
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(Rsa3005TestData.CorrectlyScopedOnly)
           .WithAnalyzer<Rsa3001>()
           .WithAnalyzer<Rsa3005>()
           .GenerateAsync();

        // Then
        result.AnalyzerResults[typeof(Rsa3001)].Diagnostics.Should().BeEmpty();
        result.AnalyzerResults[typeof(Rsa3005)].Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenWrongTargetDisposal_WhenAnalyzedWithRsa3001AndRsa3005_ThenOnlyRsa3005Fires()
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(Rsa3005TestData.Incorrect)
           .WithAnalyzer<Rsa3001>()
           .WithAnalyzer<Rsa3005>()
           .GenerateAsync();

        // Then
        result.AnalyzerResults[typeof(Rsa3001)].Diagnostics.Should().BeEmpty();
        result.AnalyzerResults[typeof(Rsa3005)].Diagnostics.Should().NotBeEmpty();
    }

    private static class Rsa3005TestData
    {
        internal const string CorrectlyScopedOnly =

            // lang=csharp
            """
            using System;
            using System.Reactive;
            using System.Reactive.Disposables;
            using System.Reactive.Linq;
            using ReactiveUI;

            namespace Sample
            {
                public class CorrectlyScopedActivatableView : IActivatableView
                {
                    public CorrectlyScopedActivatableView()
                    {
                        this.WhenActivated(d =>
                        {
                            Observable
                               .Return(Unit.Default)
                               .Subscribe()
                               .DisposeWith(d);
                        });
                    }
                }
            }
            """;

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
                public class CorrectlyScopedActivatableView : IActivatableView
                {
                    public CorrectlyScopedActivatableView()
                    {
                        this.WhenActivated(d =>
                        {
                            Observable
                               .Return(Unit.Default)
                               .Subscribe()
                               .DisposeWith(d);
                        });
                    }
                }

                public class BareSubscriptionActivatableView : IActivatableView
                {
                    public BareSubscriptionActivatableView()
                    {
                        this.WhenActivated(d =>
                        {
                            // Intentionally undisposed: no DisposeWith call exists here at all,
                            // so this is RSA3001's concern, not RSA3005's. RSA3005 only inspects
                            // DisposeWith calls that already exist, so it must stay silent here.
                            Observable
                               .Return(Unit.Default)
                               .Subscribe();
                        });
                    }
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
                public class MisscopedActivatableView : IActivatableView
                {
                    public MisscopedActivatableView()
                    {
                        this.WhenActivated(d =>
                        {
                            Observable
                               .Return(Unit.Default)
                               .Subscribe()
                               .DisposeWith(_wrongField);
                        });
                    }

                    private readonly CompositeDisposable _wrongField = new CompositeDisposable();
                }
            }
            """;
    }
}
