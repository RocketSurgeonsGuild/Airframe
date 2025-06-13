using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Usage;

public class Rsa1001Tests
{
    [Theory]
    [InlineData(Rsa1001TestData.Correct)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1001>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == Descriptions.RSA1001.Id));
    }

    [Theory]
    [InlineData(Rsa1001TestData.Incorrect)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1001>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == Descriptions.RSA1001.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa1001TestData.Correct), Rsa1001TestData.Correct)]
    [InlineData(nameof(Rsa1001TestData.Incorrect), Rsa1001TestData.Incorrect)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1001>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(typeof(Unit), typeof(ICommand), typeof(ReactiveCommand), typeof(Expression<>))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa1001TestData
    {
        internal const string Correct =

            // lang=csharp
            """
                using ReactiveUI;
                using System.Reactive;
                using System.Reactive.Linq;
                public class InvokeCommandTestData
                {
                    public InvokeCommandTestData()
                    {
                        Command = ReactiveCommand.Create(() => { });

                        Observable
                            .Return(Unit.Default)
                            .InvokeCommand(this, x => x.Command);
                    }

                    public ReactiveCommand<Unit, Unit> Command { get; }
                }
            """;

        internal const string Incorrect =

            // lang=csharp
            """
                using ReactiveUI;
                using System.Reactive;
                using System.Reactive.Linq;
                public class InvokeCommandTestData
                {
                    public InvokeCommandTestData()
                    {
                        Command = ReactiveCommand.Create(() => { });

                        Observable
                            .Return(Unit.Default)
                            .InvokeCommand(Command);
                    }

                    public ReactiveCommand<Unit, Unit> Command { get; }
                }
            """;
    }
}