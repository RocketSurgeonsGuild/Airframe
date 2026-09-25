using DynamicData;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Usage;

public class Rsa1010Tests
{
    [Theory]
    [InlineData(Rsa1010TestData.NoObserveOn)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1010>()
           .AddReferences(
                typeof(Unit),
                typeof(Expression<>),
                typeof(SourceCache<,>),
                typeof(DynamicData.Binding.ObservableCollectionExtended<>),
                typeof(System.Reactive.Linq.Observable))
           .AddReferences(
                "System.Threading.dll",
                "System.ObjectModel.dll",
                "System.ComponentModel.TypeConverter.dll")
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == Descriptions.RSA1010.Id));
    }

    [Theory]
    [InlineData(Rsa1010TestData.ObserveOnImmediatelyBefore)]
    [InlineData(Rsa1010TestData.ObserveOnEarlierInChain)]
    [InlineData(Rsa1010TestData.UnrelatedBindMethod)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1010>()
           .AddReferences(
                typeof(Unit),
                typeof(Expression<>),
                typeof(SourceCache<,>),
                typeof(DynamicData.Binding.ObservableCollectionExtended<>),
                typeof(System.Reactive.Linq.Observable))
           .AddReferences(
                "System.Threading.dll",
                "System.ObjectModel.dll",
                "System.ComponentModel.TypeConverter.dll")
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == Descriptions.RSA1010.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa1010TestData.NoObserveOn), Rsa1010TestData.NoObserveOn)]
    [InlineData(nameof(Rsa1010TestData.ObserveOnImmediatelyBefore), Rsa1010TestData.ObserveOnImmediatelyBefore)]
    [InlineData(nameof(Rsa1010TestData.ObserveOnEarlierInChain), Rsa1010TestData.ObserveOnEarlierInChain)]
    [InlineData(nameof(Rsa1010TestData.UnrelatedBindMethod), Rsa1010TestData.UnrelatedBindMethod)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1010>()
           .AddSources(source)
           .AddReferences(
                typeof(Unit),
                typeof(Expression<>),
                typeof(SourceCache<,>),
                typeof(DynamicData.Binding.ObservableCollectionExtended<>),
                typeof(System.Reactive.Linq.Observable))
           .AddReferences(
                "System.Threading.dll",
                "System.ObjectModel.dll",
                "System.ComponentModel.TypeConverter.dll")
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa1010TestData
    {
        // lang=csharp
        public const string NoObserveOn = """
            using System;
            using System.Reactive.Linq;
            using DynamicData;
            using DynamicData.Binding;

            namespace Foo.Bar;

            public class Rsa1010Example
            {
                private readonly ObservableCollectionExtended<string> _items = new();

                public Rsa1010Example(ISourceCache<string, string> changes) =>
                    changes
                       .Connect()
                       .Bind(_items)
                       .Subscribe();
            }
            """;

        // lang=csharp
        public const string ObserveOnImmediatelyBefore = """
            using System;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;
            using DynamicData;
            using DynamicData.Binding;

            namespace Foo.Bar;

            public class Rsa1010Example
            {
                private readonly ObservableCollectionExtended<string> _items = new();

                public Rsa1010Example(ISourceCache<string, string> changes, IScheduler scheduler) =>
                    changes
                       .Connect()
                       .ObserveOn(scheduler)
                       .Bind(_items)
                       .Subscribe();
            }
            """;

        // lang=csharp
        public const string ObserveOnEarlierInChain = """
            using System;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;
            using DynamicData;
            using DynamicData.Binding;

            namespace Foo.Bar;

            public class Rsa1010Example
            {
                private readonly ObservableCollectionExtended<string> _items = new();

                public Rsa1010Example(ISourceCache<string, string> changes, IScheduler scheduler) =>
                    changes
                       .Connect()
                       .ObserveOn(scheduler)
                       .Filter(value => value.Length > 0)
                       .Bind(_items)
                       .Subscribe();
            }
            """;

        // lang=csharp
        public const string UnrelatedBindMethod = """
            namespace Foo.Bar;

            public class CustomBinder
            {
                public CustomBinder Bind(string value) => this;
            }

            public class Rsa1010Example
            {
                public Rsa1010Example(CustomBinder binder) =>
                    binder.Bind("value");
            }
            """;
    }
}
