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

public class Rsa1005Tests
{
    [Theory]
    [InlineData(Rsa1005TestData.Incorrect)]
    [InlineData(Rsa1005TestData.Range)]
    [InlineData(Rsa1005TestData.DynamicDataBatch)]
    [InlineData(Rsa1005TestData.DynamicDataBatchIf)]
    [InlineData(Rsa1005TestData.DynamicDataBatchInitial)]
    [InlineData(Rsa1005TestData.CustomMethodWithScheduler)]
    [InlineData(Rsa1005TestData.MethodWithObservableParameterInOtherNamespace)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1005>()
           .AddReferences(
                typeof(Unit),
                typeof(Expression<>),
                typeof(SourceCache<,>),
                typeof(DynamicData.Binding.ObservableCollectionExtended<>),
                typeof(System.Reactive.Linq.Observable))
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == Descriptions.RSA1005.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa1005TestData.Incorrect), Rsa1005TestData.Incorrect)]
    [InlineData(nameof(Rsa1005TestData.Range), Rsa1005TestData.Range)]
    [InlineData(nameof(Rsa1005TestData.DynamicDataBatch), Rsa1005TestData.DynamicDataBatch)]
    [InlineData(nameof(Rsa1005TestData.DynamicDataBatchIf), Rsa1005TestData.DynamicDataBatchIf)]
    [InlineData(nameof(Rsa1005TestData.DynamicDataBatchInitial), Rsa1005TestData.DynamicDataBatchInitial)]
    [InlineData(nameof(Rsa1005TestData.CustomMethodWithScheduler), Rsa1005TestData.CustomMethodWithScheduler)]
    [InlineData(nameof(Rsa1005TestData.MethodWithObservableParameterInOtherNamespace), Rsa1005TestData.MethodWithObservableParameterInOtherNamespace)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1005>()
           .AddSources(source)
           .AddReferences(
                typeof(Unit),
                typeof(Expression<>),
                typeof(SourceCache<,>),
                typeof(DynamicData.Binding.ObservableCollectionExtended<>),
                typeof(System.Reactive.Linq.Observable))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa1005TestData
    {
        // lang=csharp
        public const string DynamicDataBatch = """
            using System;
            using System.Reactive;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Foo.Bar;

            public class Rsa1005Example
            {
                public Rsa1005Example(ISourceCache<string, string> changes) =>
                    changes
                       .Connect()
                       .Batch(TimeSpan.FromSeconds(1))
                       .Subscribe();
            }
            """;

        // lang=csharp
        public const string DynamicDataBatchIf = """
            using System;
            using System.Reactive;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Foo.Bar;

            public class Rsa1005Example
            {
                public Rsa1005Example(ISourceCache<string, string> changes, IObservable<bool> canBatch) =>
                    changes
                       .Connect()
                       .BatchIf(canBatch, true, TimeSpan.FromSeconds(1))
                       .Subscribe();
            }
            """;

        // lang=csharp
        public const string DynamicDataBatchInitial = """
            using System;
            using System.Reactive;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Foo.Bar;

            public class Rsa1005Example
            {
                public Rsa1005Example(ISourceCache<string, string> changes) =>
                    changes
                       .Connect()
                       .BufferInitial(TimeSpan.FromSeconds(1))
                       .Subscribe();
            }
            """;

        // lang=csharp
        public const string CustomMethodWithScheduler = """
            using System;
            using System.Reactive;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;

            namespace Foo.Bar;
            
            public static class ObservableExtensions
            {
                public static IObservable<Unit> CustomOperator(this IObservable<Unit> source) => source;
                public static IObservable<Unit> CustomOperator(this IObservable<Unit> source, IScheduler scheduler) => source;
            }

            public class Rsa1005Example
            {
                public Rsa1005Example(IObservable<Unit> source) =>
                    source
                       .CustomOperator()
                       .Subscribe();
            }
            """;

        // lang=csharp
        public const string Range = """
            using System;
            using System.Reactive;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;

            namespace Foo.Bar;

            public class Rsa1005Example
            {
                public Rsa1005Example() =>
                    Observable
                       .Range(0, 10)
                       .Subscribe();
            }
            """;

        // lang=csharp
        public const string Incorrect = """
            using System;
            using System.Reactive;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;

            namespace Foo.Bar;

            public class Rsa1005Example
            {
                public Rsa1005Example() =>
                    Observable
                       .Return(Unit.Default)
                       .Throttle(TimeSpan.Zero)
                       .Subscribe();
            }
            """;

        // lang=csharp
        public const string MethodWithObservableParameterInOtherNamespace = """
            using System;
            using System.Reactive;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;

            namespace OtherNamespace
            {
                public static class OtherExtensions
                {
                    public static IObservable<Unit> OtherOperator(this IObservable<Unit> source) => source;
                    public static IObservable<Unit> OtherOperator(this IObservable<Unit> source, IScheduler scheduler) => source;
                }
            }

            namespace Foo.Bar
            {
                using OtherNamespace;

                public class Rsa1005Example
                {
                    public Rsa1005Example(IObservable<Unit> source) =>
                        source
                           .OtherOperator()
                           .Subscribe();
                }
            }
            """;
    }
}