using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;
using Rocket.Surgery.Airframe.CodeFixes.Performance;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Performance;

public class Rsa3006FixTests
{
    [Theory]
    [InlineData(nameof(Rsa3006FixTestData.ConversionLocalFunction), Rsa3006FixTestData.ConversionLocalFunction)]
    [InlineData(nameof(Rsa3006FixTestData.ConversionExpression), Rsa3006FixTestData.ConversionExpression)]
    [InlineData(nameof(Rsa3006FixTestData.FromEventAction), Rsa3006FixTestData.FromEventAction)]
    [InlineData(nameof(Rsa3006FixTestData.FromEventUnit), Rsa3006FixTestData.FromEventUnit)]
    [InlineData(nameof(Rsa3006FixTestData.FromEventTypedDelegate), Rsa3006FixTestData.FromEventTypedDelegate)]
    [InlineData(nameof(Rsa3006FixTestData.PatternDelegate), Rsa3006FixTestData.PatternDelegate)]
    [InlineData(nameof(Rsa3006FixTestData.PatternEventHandlerOfT), Rsa3006FixTestData.PatternEventHandlerOfT)]
    [InlineData(nameof(Rsa3006FixTestData.PatternEventHandler), Rsa3006FixTestData.PatternEventHandler)]
    [InlineData(nameof(Rsa3006FixTestData.PatternSenderAndArgs), Rsa3006FixTestData.PatternSenderAndArgs)]
    [InlineData(nameof(Rsa3006FixTestData.PatternConversion), Rsa3006FixTestData.PatternConversion)]
    [InlineData(nameof(Rsa3006FixTestData.ReflectionInstance), Rsa3006FixTestData.ReflectionInstance)]
    [InlineData(nameof(Rsa3006FixTestData.ReflectionThisLiteral), Rsa3006FixTestData.ReflectionThisLiteral)]
    [InlineData(nameof(Rsa3006FixTestData.ReflectionStatic), Rsa3006FixTestData.ReflectionStatic)]
    [InlineData(nameof(Rsa3006FixTestData.NameCollisions), Rsa3006FixTestData.NameCollisions)]
    [InlineData(nameof(Rsa3006FixTestData.MissingUsings), Rsa3006FixTestData.MissingUsings)]
    [InlineData(nameof(Rsa3006FixTestData.HandlerPassedToMethod), Rsa3006FixTestData.HandlerPassedToMethod)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await Fix(source);

        // Then
        await Verifier.Verify(result).IgnoreMembers("TextChanges").HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    [Theory]
    [InlineData(Rsa3006FixTestData.ConversionLocalFunction)]
    [InlineData(Rsa3006FixTestData.ConversionExpression)]
    [InlineData(Rsa3006FixTestData.FromEventAction)]
    [InlineData(Rsa3006FixTestData.FromEventUnit)]
    [InlineData(Rsa3006FixTestData.FromEventTypedDelegate)]
    [InlineData(Rsa3006FixTestData.PatternDelegate)]
    [InlineData(Rsa3006FixTestData.PatternEventHandlerOfT)]
    [InlineData(Rsa3006FixTestData.PatternEventHandler)]
    [InlineData(Rsa3006FixTestData.PatternSenderAndArgs)]
    [InlineData(Rsa3006FixTestData.PatternConversion)]
    [InlineData(Rsa3006FixTestData.ReflectionInstance)]
    [InlineData(Rsa3006FixTestData.ReflectionThisLiteral)]
    [InlineData(Rsa3006FixTestData.ReflectionStatic)]
    [InlineData(Rsa3006FixTestData.NameCollisions)]
    [InlineData(Rsa3006FixTestData.MissingUsings)]
    [InlineData(Rsa3006FixTestData.HandlerPassedToMethod)]
    public async Task GivenSource_WhenCodeFix_ThenFixedCodeCompilesWithoutFromEvent(string source)
    {
        // Given, When
        var result = await Fix(source);

        // Then
        var actions = result.CodeFixResults[typeof(Rsa3006Fix)].ResolvedFixes.SelectMany(fix => fix.CodeActions).ToList();
        actions.Should().ContainSingle();

        var changes = actions[0].Changes;
        var document = changes.NewProject.GetDocument(changes.GetChangedDocuments().Single())!;
        var text = (await document.GetTextAsync()).ToString();
        var compilation = await document.Project.GetCompilationAsync();

        text.Should().Contain("Observable.Create<").And.NotContain("FromEvent");
        compilation!
           .GetDiagnostics()
           .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
           .Should()
           .BeEmpty(because: text);
    }

    [Theory]
    [InlineData(Rsa3006FixTestData.WithScheduler)]
    [InlineData(Rsa3006FixTestData.MethodGroupHandlers)]
    [InlineData(Rsa3006FixTestData.ReflectionUnknownEvent)]
    [InlineData(Rsa3006FixTestData.ReflectionUnstableReceiver)]
    [InlineData(Rsa3006FixTestData.ConversionMethodGroup)]
    public async Task GivenUnfixableSource_WhenCodeFix_ThenNoCodeActionOffered(string source)
    {
        // Given, When
        var result = await Fix(source);

        // Then
        result.AnalyzerResults[typeof(Rsa3006)].Diagnostics.Should().ContainSingle();
        result
           .CodeFixResults[typeof(Rsa3006Fix)]
           .ResolvedFixes
           .SelectMany(fix => fix.CodeActions)
           .Should()
           .BeEmpty();
    }

    private static Task<GeneratorTestResults> Fix(string source) =>
        GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa3006>()
           .WithCodeFix<Rsa3006Fix>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(
                typeof(Unit),
                typeof(INotifyPropertyChanged),
                typeof(System.Reactive.Linq.Observable))
           .GenerateAsync();
}

internal static class Rsa3006FixTestData
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

                public event System.EventHandler<string> Named;

                public event System.Action<int> Counted;

                public event System.Action Pinged;
            }
        }
        """;

    // lang=csharp
    internal const string ConversionLocalFunction = """
        using System;
        using System.ComponentModel;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                public IObservable<PropertyChangingEventArgs> Changing(Source source) =>
                    Observable.FromEvent<PropertyChangingEventHandler, PropertyChangingEventArgs>(handler =>
                                                                                                 {
                                                                                                     void Handler(object sender, PropertyChangingEventArgs args) => handler(args);

                                                                                                     return Handler;
                                                                                                 }, handler => source.PropertyChanging += handler,
                                                                                                 handler => source.PropertyChanging -= handler);
            }
        }
        """ + Source;

    // lang=csharp
    internal const string ConversionExpression = """
        using System;
        using System.ComponentModel;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                public IObservable<string> Names(Source source) =>
                    Observable.FromEvent<PropertyChangedEventHandler, string>(
                        handler => (sender, args) => handler(args.PropertyName),
                        handler => source.PropertyChanged += handler,
                        handler => source.PropertyChanged -= handler);
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
    internal const string PatternDelegate = """
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
    internal const string PatternEventHandlerOfT = """
        using System;
        using System.Reactive;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                public IObservable<EventPattern<string>> Named(Source source) =>
                    Observable.FromEventPattern<string>(h => source.Named += h, h => source.Named -= h);
            }
        }
        """ + Source;

    // lang=csharp
    internal const string PatternEventHandler = """
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
    internal const string PatternSenderAndArgs = """
        using System;
        using System.ComponentModel;
        using System.Reactive;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                public IObservable<EventPattern<Source, PropertyChangedEventArgs>> Changed(Source source) =>
                    Observable.FromEventPattern<PropertyChangedEventHandler, Source, PropertyChangedEventArgs>(
                        h => source.PropertyChanged += h,
                        h => source.PropertyChanged -= h);
            }
        }
        """ + Source;

    // lang=csharp
    internal const string PatternConversion = """
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
                        h => (sender, e) => h(sender, e),
                        h => source.PropertyChanged += h,
                        h => source.PropertyChanged -= h);
            }
        }
        """ + Source;

    // lang=csharp
    internal const string ReflectionInstance = """
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
    internal const string ReflectionThisLiteral = """
        using System;
        using System.Reactive;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                public event EventHandler Changed;

                public IObservable<EventPattern<object>> Changes() => Observable.FromEventPattern(this, "Changed");
            }
        }
        """ + Source;

    // lang=csharp
    internal const string ReflectionStatic = """
        using System;
        using System.Reactive;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                public IObservable<EventPattern<object>> Changed() =>
                    Observable.FromEventPattern(typeof(Source), nameof(Source.StaticChanged));
            }
        }
        """ + Source;

    // lang=csharp
    internal const string NameCollisions = """
        using System;
        using System.Reactive;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                public IDisposable Changed(Source source, object sender, EventArgs e)
                {
                    var observer = Observer.Create<EventPattern<object>>(_ => { });
                    var Handler = 1;

                    return Observable
                       .FromEventPattern(h => source.Changed += h, h => source.Changed -= h)
                       .Subscribe(observer);
                }
            }
        }
        """ + Source;

    // lang=csharp
    internal const string MissingUsings = """
        namespace Sample
        {
            public class Example
            {
                public System.IObservable<int> Counted(Source source) =>
                    System.Reactive.Linq.Observable.FromEvent<int>(
                        h =>
                        {
                            source.Counted += h;
                        },
                        h =>
                        {
                            source.Counted -= h;
                        });
            }
        }
        """ + Source;

    // lang=csharp
    internal const string HandlerPassedToMethod = """
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
                        h => Register(source, h),
                        h => Unregister(source, h));

                private static void Register(Source source, Delegate handler) =>
                    source.PropertyChanged += (PropertyChangedEventHandler)handler;

                private static void Unregister(Source source, Delegate handler) =>
                    source.PropertyChanged -= (PropertyChangedEventHandler)handler;
            }
        }
        """ + Source;

    // lang=csharp
    internal const string WithScheduler = """
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
    internal const string MethodGroupHandlers = """
        using System;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                private readonly Source _source = new();

                public IObservable<int> Counted() => Observable.FromEvent<int>(Add, Remove);

                private void Add(Action<int> handler) => _source.Counted += handler;

                private void Remove(Action<int> handler) => _source.Counted -= handler;
            }
        }
        """ + Source;

    // lang=csharp
    internal const string ReflectionUnknownEvent = """
        using System;
        using System.Reactive;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                public IObservable<EventPattern<object>> Missing(Source source) => Observable.FromEventPattern(source, "Missing");
            }
        }
        """ + Source;

    // lang=csharp
    internal const string ReflectionUnstableReceiver = """
        using System;
        using System.Reactive;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                public IObservable<EventPattern<object>> Changed() => Observable.FromEventPattern(Create(), nameof(Source.Changed));

                private static Source Create() => new();
            }
        }
        """ + Source;

    // lang=csharp
    internal const string ConversionMethodGroup = """
        using System;
        using System.Reactive.Linq;

        namespace Sample
        {
            public class Example
            {
                public IObservable<int> Counted(Source source) =>
                    Observable.FromEvent<Action<int>, int>(Convert, h => source.Counted += h, h => source.Counted -= h);

                private static Action<int> Convert(Action<int> handler) => handler;
            }
        }
        """ + Source;
}
