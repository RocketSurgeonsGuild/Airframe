using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;
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

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Usage;

public class Rsa1008Tests
{
    [Theory]
    [InlineData(Rsa1008TestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1008>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA1008.Id));
    }

    [Theory]
    [InlineData(Rsa1008TestData.Incorrect)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1008>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == RSA1008.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa1008TestData.Correct), Rsa1008TestData.Correct)]
    [InlineData(nameof(Rsa1008TestData.Incorrect), Rsa1008TestData.Incorrect)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1008>()
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

    private static class Rsa1008TestData
    {
        internal const string Correct =

            // lang=csharp
            """
            using System;
            using ReactiveUI;

            namespace Sample
            {
                public class RaiseAndSetIfChangedExample : ReactiveObject
                {
                    public string Name
                    {
                        get => _name;
                        set => this.RaiseAndSetIfChanged(ref _name, value);
                    }

                    private string _name;
                }

                public class AutoPropertyExample : ReactiveObject
                {
                    // Auto-properties not raising notification is a real but different bug
                    // (missing [Reactive] attribute) - explicitly out of scope for RSA1008.
                    public string Name { get; set; }
                }

                public class PlainClassExample
                {
                    // Not a ReactiveObject, so a direct field assignment here is a generic C#
                    // pattern, not a ReactiveUI misuse - RSA1008 must not flag it.
                    public string Name
                    {
                        get => _name;
                        set => _name = value;
                    }

                    private string _name;
                }

                public class InitOnlyExample : ReactiveObject
                {
                    // A property set only at construction/object-initializer time has no
                    // meaningful "notify subscribers of a later change" concern.
                    public string Name
                    {
                        get => _name;
                        init => _name = value;
                    }

                    private string _name;
                }
            }
            """;

        internal const string Incorrect =

            // lang=csharp
            """
            using System;
            using ReactiveUI;

            namespace Sample
            {
                public class DirectAssignmentArrowExample : ReactiveObject
                {
                    public string Name
                    {
                        get => _name;
                        set => _name = value;
                    }

                    private string _name;
                }

                public class DirectAssignmentBlockExample : ReactiveObject
                {
                    public string Name
                    {
                        get { return _name; }
                        set { _name = value; }
                    }

                    private string _name;
                }
            }
            """;
    }
}
