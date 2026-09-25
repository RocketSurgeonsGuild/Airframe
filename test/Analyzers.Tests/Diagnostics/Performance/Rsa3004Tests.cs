using DynamicData;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;
using VerifyXunit;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Performance;

public class Rsa3004Tests
{
    [Theory]
    [InlineData(Rsa3004TestData.Correct)]
    [InlineData(Rsa3004TestData.CorrectNoSelectorOverload)]
    [InlineData(Rsa3004TestData.UnrelatedAutoRefresh)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3004>()
           .AddReferences(
                typeof(Unit),
                typeof(System.IServiceProvider),
                typeof(System.ComponentModel.INotifyPropertyChanged),
                typeof(Expression<>),
                typeof(SourceCache<,>),
                typeof(DynamicData.Binding.ObservableCollectionExtended<>),
                typeof(System.Reactive.Linq.Observable))
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA3004.Id));
    }

    [Theory]
    [InlineData(Rsa3004TestData.Incorrect)]
    [InlineData(Rsa3004TestData.IncorrectNoSelectorOverload)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3004>()
           .AddReferences(
                typeof(Unit),
                typeof(System.IServiceProvider),
                typeof(System.ComponentModel.INotifyPropertyChanged),
                typeof(Expression<>),
                typeof(SourceCache<,>),
                typeof(DynamicData.Binding.ObservableCollectionExtended<>),
                typeof(System.Reactive.Linq.Observable))
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA3004.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa3004TestData.Correct), Rsa3004TestData.Correct)]
    [InlineData(nameof(Rsa3004TestData.Incorrect), Rsa3004TestData.Incorrect)]
    [InlineData(nameof(Rsa3004TestData.CorrectNoSelectorOverload), Rsa3004TestData.CorrectNoSelectorOverload)]
    [InlineData(nameof(Rsa3004TestData.IncorrectNoSelectorOverload), Rsa3004TestData.IncorrectNoSelectorOverload)]
    [InlineData(nameof(Rsa3004TestData.UnrelatedAutoRefresh), Rsa3004TestData.UnrelatedAutoRefresh)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3004>()
           .AddSources(source)
           .AddReferences(
                typeof(Unit),
                typeof(System.IServiceProvider),
                typeof(System.ComponentModel.INotifyPropertyChanged),
                typeof(Expression<>),
                typeof(SourceCache<,>),
                typeof(DynamicData.Binding.ObservableCollectionExtended<>),
                typeof(System.Reactive.Linq.Observable))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa3004TestData
    {
        // lang=csharp
        internal const string Correct = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3004Example
                {
                    public Rsa3004Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh(x => x.Thing, scheduler: TaskPoolScheduler.Default)
                           .Subscribe();
                }

                public class Item : INotifyPropertyChanged
                {
                    public string Id { get; set; } = string.Empty;

                    private string _thing = string.Empty;

                    public string Thing
                    {
                        get => _thing;
                        set
                        {
                            _thing = value;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thing)));
                        }
                    }

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }
            """;

        // lang=csharp
        internal const string Incorrect = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3004Example
                {
                    public Rsa3004Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh(x => x.Thing)
                           .Subscribe();
                }

                public class Item : INotifyPropertyChanged
                {
                    public string Id { get; set; } = string.Empty;

                    private string _thing = string.Empty;

                    public string Thing
                    {
                        get => _thing;
                        set
                        {
                            _thing = value;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thing)));
                        }
                    }

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }
            """;

        // lang=csharp
        internal const string CorrectNoSelectorOverload = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3004Example
                {
                    public Rsa3004Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh(scheduler: TaskPoolScheduler.Default)
                           .Subscribe();
                }

                public class Item : INotifyPropertyChanged
                {
                    public string Id { get; set; } = string.Empty;

                    private string _thing = string.Empty;

                    public string Thing
                    {
                        get => _thing;
                        set
                        {
                            _thing = value;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thing)));
                        }
                    }

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }
            """;

        // lang=csharp
        internal const string IncorrectNoSelectorOverload = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3004Example
                {
                    public Rsa3004Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh()
                           .Subscribe();
                }

                public class Item : INotifyPropertyChanged
                {
                    public string Id { get; set; } = string.Empty;

                    private string _thing = string.Empty;

                    public string Thing
                    {
                        get => _thing;
                        set
                        {
                            _thing = value;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thing)));
                        }
                    }

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }
            """;

        // lang=csharp
        internal const string UnrelatedAutoRefresh = """
            namespace Sample
            {
                public static class NonReactiveExtensions
                {
                    public static void AutoRefresh(this int value)
                    {
                    }
                }

                public class Rsa3004Example
                {
                    public Rsa3004Example() => 5.AutoRefresh();
                }
            }
            """;
    }
}
