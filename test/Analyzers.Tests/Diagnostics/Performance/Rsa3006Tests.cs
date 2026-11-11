using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using VerifyXunit;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Performance;

public class Rsa3006Tests
{
    [Theory]
    [InlineData(Rsa3006TestData.ObservableCreate)]
    [InlineData(Rsa3006TestData.OwnObservableType)]
    [InlineData(Rsa3006TestData.InstanceFromEventMethod)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await Analyze(source);

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA3006.Id));
    }

    [Theory]
    [InlineData(Rsa3006TestData.FromEventConversion)]
    [InlineData(Rsa3006TestData.FromEventTypedDelegate)]
    [InlineData(Rsa3006TestData.FromEventAction)]
    [InlineData(Rsa3006TestData.FromEventUnit)]
    [InlineData(Rsa3006TestData.FromEventPatternDelegate)]
    [InlineData(Rsa3006TestData.FromEventPatternEventHandler)]
    [InlineData(Rsa3006TestData.FromEventPatternReflection)]
    [InlineData(Rsa3006TestData.FromEventPatternStaticReflection)]
    [InlineData(Rsa3006TestData.FromEventWithScheduler)]
    [InlineData(Rsa3006TestData.UsingStatic)]
    public async Task GivenIncorrect_WhenAnalyze_ThenSingleDiagnosticReported(string source)
    {
        // Given, When
        var result = await Analyze(source);

        // Then
        result
           .AnalyzerResults
           .SelectMany(pair => pair.Value.Diagnostics)
           .Where(diagnostic => diagnostic.Id == RSA3006.Id)
           .Should()
           .ContainSingle();
    }

    [Theory]
    [InlineData(nameof(Rsa3006TestData.FromEventConversion), Rsa3006TestData.FromEventConversion)]
    [InlineData(nameof(Rsa3006TestData.FromEventPatternReflection), Rsa3006TestData.FromEventPatternReflection)]
    [InlineData(nameof(Rsa3006TestData.ObservableCreate), Rsa3006TestData.ObservableCreate)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await Analyze(source);

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static Task<GeneratorTestResults> Analyze(string source) =>
        GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3006>()
           .AddSources(source)
           .AddReferences(
                typeof(Unit),
                typeof(INotifyPropertyChanged),
                typeof(System.Reactive.Linq.Observable))
           .GenerateAsync();

    private static class Rsa3006TestData
    {
        private const string Source =

            // lang=csharp
            """

            #pragma warning disable CS0067
            namespace Sample
            {
                public class Source : System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.INotifyPropertyChanging
                {
                    public static event System.EventHandler StaticChanged;

                    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

                    public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

                    public event System.EventHandler Changed;

                    public event System.Action<int> Counted;

                    public event System.Action Pinged;
                }
            }
            """;

        // lang=csharp
        internal const string FromEventConversion = """
            using System;
            using System.ComponentModel;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<PropertyChangingEventArgs> Changing(Source source) =>
                        Observable.FromEvent<PropertyChangingEventHandler, PropertyChangingEventArgs>(
                            handler =>
                            {
                                void Handler(object sender, PropertyChangingEventArgs args) => handler(args);

                                return Handler;
                            },
                            handler => source.PropertyChanging += handler,
                            handler => source.PropertyChanging -= handler);
                }
            }
            """ + Source;

        // lang=csharp
        internal const string FromEventTypedDelegate = """
            using System;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<int> Counted(Source source) =>
                        Observable.FromEvent<Action<int>, int>(h => source.Counted += h, h => source.Counted -= h);
                }
            }
            """ + Source;

        // lang=csharp
        internal const string FromEventAction = """
            using System;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<int> Counted(Source source) =>
                        Observable.FromEvent<int>(h => source.Counted += h, h => source.Counted -= h);
                }
            }
            """ + Source;

        // lang=csharp
        internal const string FromEventUnit = """
            using System;
            using System.Reactive;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<Unit> Pinged(Source source) =>
                        Observable.FromEvent(h => source.Pinged += h, h => source.Pinged -= h);
                }
            }
            """ + Source;

        // lang=csharp
        internal const string FromEventPatternDelegate = """
            using System;
            using System.ComponentModel;
            using System.Reactive;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<EventPattern<PropertyChangedEventArgs>> Changed(Source source) =>
                        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                            h => source.PropertyChanged += h,
                            h => source.PropertyChanged -= h);
                }
            }
            """ + Source;

        // lang=csharp
        internal const string FromEventPatternEventHandler = """
            using System;
            using System.Reactive;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<EventPattern<object>> Changed(Source source) =>
                        Observable.FromEventPattern(h => source.Changed += h, h => source.Changed -= h);
                }
            }
            """ + Source;

        // lang=csharp
        internal const string FromEventPatternReflection = """
            using System;
            using System.ComponentModel;
            using System.Reactive;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<EventPattern<PropertyChangedEventArgs>> Changed(Source source) =>
                        Observable.FromEventPattern<PropertyChangedEventArgs>(source, nameof(Source.PropertyChanged));
                }
            }
            """ + Source;

        // lang=csharp
        internal const string FromEventPatternStaticReflection = """
            using System;
            using System.Reactive;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<EventPattern<object>> Changed() =>
                        Observable.FromEventPattern(typeof(Source), "StaticChanged");
                }
            }
            """ + Source;

        // lang=csharp
        internal const string FromEventWithScheduler = """
            using System;
            using System.Reactive.Concurrency;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<int> Counted(Source source) =>
                        Observable.FromEvent<int>(h => source.Counted += h, h => source.Counted -= h, ImmediateScheduler.Instance);
                }
            }
            """ + Source;

        // lang=csharp
        internal const string UsingStatic = """
            using System;
            using static System.Reactive.Linq.Observable;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<int> Counted(Source source) =>
                        FromEvent<int>(h => source.Counted += h, h => source.Counted -= h);
                }
            }
            """ + Source;

        // lang=csharp
        internal const string ObservableCreate = """
            using System;
            using System.Reactive.Disposables;
            using System.Reactive.Linq;

            namespace Sample
            {
                public class Example
                {
                    public IObservable<int> Counted(Source source) =>
                        Observable.Create<int>(observer =>
                        {
                            void Handler(int args) => observer.OnNext(args);
                            source.Counted += Handler;
                            return Disposable.Create(() => source.Counted -= Handler);
                        });
                }
            }
            """ + Source;

        // lang=csharp
        internal const string OwnObservableType = """
            using System;

            namespace Sample
            {
                public static class Observable
                {
                    public static IObservable<T> FromEvent<T>(Action<Action<T>> add, Action<Action<T>> remove) => null;
                }

                public class Example
                {
                    public IObservable<int> Counted(Source source) =>
                        Observable.FromEvent<int>(h => source.Counted += h, h => source.Counted -= h);
                }
            }
            """ + Source;

        // lang=csharp
        internal const string InstanceFromEventMethod = """
            using System;

            namespace Sample
            {
                public class Example
                {
                    public void FromEventPattern(string name)
                    {
                    }

                    public void Run() => FromEventPattern("Changed");
                }
            }
            """ + Source;
    }
}
