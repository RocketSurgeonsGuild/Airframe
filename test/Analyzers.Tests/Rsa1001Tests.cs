using FluentAssertions;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe.Analyzers.Tests;

public class Rsa1001Tests
{
    [Theory]
    [InlineData(Rsa1001TestData.Correct)]
    public async Task GivenCorrect_When_Then(string source)
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
    public async Task GivenIncorrect_When_Then(string source)
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
}

internal static class Rsa1001TestData
{
    internal const string Correct = @"
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
    }";

    internal const string Incorrect = @"
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
    }";
}