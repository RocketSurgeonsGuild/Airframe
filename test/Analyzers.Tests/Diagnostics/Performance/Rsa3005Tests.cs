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

public class Rsa3005Tests
{
    [Theory]
    [InlineData(Rsa3005TestData.SinglePropertyOnce)]
    [InlineData(Rsa3005TestData.DifferentProperties)]
    [InlineData(Rsa3005TestData.ConditionalExpressionSelector)]
    [InlineData(Rsa3005TestData.MethodCallSelector)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3005>()
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
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA3005.Id));
    }

    [Theory]
    [InlineData(Rsa3005TestData.SamePropertyTwice)]
    [InlineData(Rsa3005TestData.NestedPropertyPathTwice)]
    [InlineData(Rsa3005TestData.NamedReorderedArgumentsSamePropertyTwice)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa3005>()
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
           .AnalyzerResults[typeof(Rsa3005)]
           .Diagnostics
           .Should()
           .ContainSingle(diagnostic => diagnostic.Id == RSA3005.Id);
    }

    [Theory]
    [InlineData(nameof(Rsa3005TestData.SinglePropertyOnce), Rsa3005TestData.SinglePropertyOnce)]
    [InlineData(nameof(Rsa3005TestData.DifferentProperties), Rsa3005TestData.DifferentProperties)]
    [InlineData(nameof(Rsa3005TestData.SamePropertyTwice), Rsa3005TestData.SamePropertyTwice)]
    [InlineData(nameof(Rsa3005TestData.NestedPropertyPathTwice), Rsa3005TestData.NestedPropertyPathTwice)]
    [InlineData(nameof(Rsa3005TestData.ConditionalExpressionSelector), Rsa3005TestData.ConditionalExpressionSelector)]
    [InlineData(nameof(Rsa3005TestData.MethodCallSelector), Rsa3005TestData.MethodCallSelector)]
    [InlineData(nameof(Rsa3005TestData.NamedReorderedArgumentsSamePropertyTwice), Rsa3005TestData.NamedReorderedArgumentsSamePropertyTwice)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3005>()
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

    private static class Rsa3005TestData
    {
        // lang=csharp
        internal const string SinglePropertyOnce = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3005Example
                {
                    public Rsa3005Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh(x => x.Thing)
                           .Subscribe();
                }

                public class Item : INotifyPropertyChanged
                {
                    public string Id { get; set; } = string.Empty;

                    private string _thing = string.Empty;
                    private string _otherThing = string.Empty;

                    public string Thing
                    {
                        get => _thing;
                        set
                        {
                            _thing = value;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thing)));
                        }
                    }

                    public string OtherThing
                    {
                        get => _otherThing;
                        set
                        {
                            _otherThing = value;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OtherThing)));
                        }
                    }

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }
            """;

        // lang=csharp
        internal const string DifferentProperties = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3005Example
                {
                    public Rsa3005Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh(x => x.Thing)
                           .AutoRefresh(x => x.OtherThing)
                           .Subscribe();
                }

                public class Item : INotifyPropertyChanged
                {
                    public string Id { get; set; } = string.Empty;

                    private string _thing = string.Empty;
                    private string _otherThing = string.Empty;

                    public string Thing
                    {
                        get => _thing;
                        set
                        {
                            _thing = value;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thing)));
                        }
                    }

                    public string OtherThing
                    {
                        get => _otherThing;
                        set
                        {
                            _otherThing = value;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OtherThing)));
                        }
                    }

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }
            """;

        // lang=csharp
        internal const string SamePropertyTwice = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3005Example
                {
                    public Rsa3005Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh(x => x.Thing)
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

        // Proves GetMemberPath's normalization walks the full member-access chain (not just the last
        // segment), so a nested accessor repeated further down the chain is still recognized as a duplicate.
        // lang=csharp
        internal const string NestedPropertyPathTwice = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3005Example
                {
                    public Rsa3005Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh(x => x.Foo.Bar)
                           .AutoRefresh(x => x.Foo.Bar)
                           .Subscribe();
                }

                public class Item : INotifyPropertyChanged
                {
                    public string Id { get; set; } = string.Empty;

                    public SubItem Foo { get; set; } = new();

                    public event PropertyChangedEventHandler PropertyChanged;
                }

                public class SubItem
                {
                    public string Bar { get; set; } = string.Empty;
                }
            }
            """;

        // False-negative guard: a conditional-expression selector body isn't a plain member-access chain,
        // so GetMemberPath can't confidently normalize it and must not report a duplicate even though both
        // calls use the same source expression.
        // lang=csharp
        internal const string ConditionalExpressionSelector = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3005Example
                {
                    public Rsa3005Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh(x => x.Flag ? x.Thing : x.OtherThing)
                           .AutoRefresh(x => x.Flag ? x.Thing : x.OtherThing)
                           .Subscribe();
                }

                public class Item : INotifyPropertyChanged
                {
                    public string Id { get; set; } = string.Empty;

                    public bool Flag { get; set; }

                    private string _thing = string.Empty;
                    private string _otherThing = string.Empty;

                    public string Thing
                    {
                        get => _thing;
                        set
                        {
                            _thing = value;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thing)));
                        }
                    }

                    public string OtherThing
                    {
                        get => _otherThing;
                        set
                        {
                            _otherThing = value;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OtherThing)));
                        }
                    }

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }
            """;

        // False-negative guard: a method-call selector body isn't a plain member-access chain either, so it
        // must not be treated as comparable — false negatives here are the intended, safer failure mode.
        // lang=csharp
        internal const string MethodCallSelector = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3005Example
                {
                    public Rsa3005Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh(x => x.GetThing())
                           .AutoRefresh(x => x.GetThing())
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

                    public string GetThing() => Thing;

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }
            """;

        // Proves the selector argument is now located by semantic parameter binding rather than syntactic
        // position: both calls pass their arguments named and reordered, and the duplicate is still caught.
        // lang=csharp
        internal const string NamedReorderedArgumentsSamePropertyTwice = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;
            using DynamicData;

            namespace Sample
            {
                public class Rsa3005Example
                {
                    public Rsa3005Example(SourceCache<Item, string> cache) =>
                        cache
                           .Connect()
                           .AutoRefresh(scheduler: TaskPoolScheduler.Default, propertyAccessor: x => x.Thing)
                           .AutoRefresh(scheduler: TaskPoolScheduler.Default, propertyAccessor: x => x.Thing)
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
    }
}
