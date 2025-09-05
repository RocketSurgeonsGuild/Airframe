using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Splat;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Usage;

public class Rsa1003Tests
{
    [Theory]
    [InlineData(Rsa1003TestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1003>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == Descriptions.RSA1003.Id));
    }

    [Theory]
    [InlineData(Rsa1003TestData.Incorrect)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1003>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == Descriptions.RSA1003.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa1003TestData.Correct), Rsa1003TestData.Correct)]
    [InlineData(nameof(Rsa1003TestData.Incorrect), Rsa1003TestData.Incorrect)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1003>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(typeof(Unit), typeof(ICommand), typeof(ReactiveCommand), typeof(Expression<>), typeof(IEnableLogger))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa1003TestData
    {
        internal const string Correct =

            // lang=csharp
            """
            using ReactiveUI;
            using System.Reactive;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class OutParameterAssignmentExample : ReactiveObject
                {
                    public OutParameterAssignmentExample()
                    {
                            Observable
                               .Return(Unit.Default)
                               .ToProperty(this, nameof(Value), out _value);
                    }

                    public Unit Value => _value.Value;

                    private readonly ObservableAsPropertyHelper<Unit> _value = ObservableAsPropertyHelper<Unit>.Default();
                }
            }
            """;

        internal const string Incorrect =

            // lang=csharp
            """
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
            }
            """;
    }
}