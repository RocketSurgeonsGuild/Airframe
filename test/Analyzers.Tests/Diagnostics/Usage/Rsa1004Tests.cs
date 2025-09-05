// <copyright file="Rsa1004Tests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Splat;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Usage;

public class Rsa1004Tests
{
    [Theory]
    [InlineData(Rsa1004TestData.CorrectSingleProperty)]
    [InlineData(Rsa1004TestData.CorrectMultipleProperty)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1004>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == Descriptions.RSA1004.Id));
    }

    [Theory]
    [InlineData(Rsa1004TestData.IncorrectSingleProperty)]
    [InlineData(Rsa1004TestData.IncorrectMultipleProperty)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given. When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa1004>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == Descriptions.RSA1004.Id));
    }

    [Theory]
    [InlineData(nameof(Rsa1004TestData.CorrectSingleProperty), Rsa1004TestData.CorrectSingleProperty)]
    [InlineData(nameof(Rsa1004TestData.CorrectMultipleProperty), Rsa1004TestData.CorrectMultipleProperty)]
    [InlineData(nameof(Rsa1004TestData.IncorrectSingleProperty), Rsa1004TestData.IncorrectSingleProperty)]
    [InlineData(nameof(Rsa1004TestData.IncorrectMultipleProperty), Rsa1004TestData.IncorrectMultipleProperty)]
    public async Task GivenSource_WhenAnalyze_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1004>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(typeof(Unit), typeof(ICommand), typeof(ReactiveCommand), typeof(Expression<>), typeof(IEnableLogger))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    private static class Rsa1004TestData
    {
        internal const string CorrectSingleProperty =

            // lang=csharp
            """
            using System;
            using System.Reactive;
            using ReactiveUI;

            namespace Sample
            {
                public class WhenAnyValueClosureExample : ReactiveObject
                {
                    public WhenAnyValueClosureExample() => this.WhenAnyValue(x => x.Value).Subscribe();

                    public Unit Value
                    {
                        get => _value;
                        set => this.RaiseAndSetIfChanged(ref _value, value);
                    }

                    private Unit _value;
                }
            }
            """;

        internal const string IncorrectSingleProperty =

            // lang=csharp
            """
            using System;
            using System.Reactive;
            using ReactiveUI;

            namespace Sample
            {
                public class WhenAnyValueClosureExample : ReactiveObject
                {
                    public WhenAnyValueClosureExample() => this.WhenAnyValue(x => Value).Subscribe();

                    public Unit Value
                    {
                        get => _value;
                        set => this.RaiseAndSetIfChanged(ref _value, value);
                    }

                    private Unit _value;
                }
            }
            """;

        internal const string CorrectMultipleProperty =

            // lang=csharp
            """
            using System;
            using System.Reactive;
            using ReactiveUI;

            namespace Sample
            {
                public class WhenAnyValueClosureExample : ReactiveObject
                {
                    public WhenAnyValueClosureExample() => this.WhenAnyValue(x => x.Value, y => y.Value).Subscribe();

                    public Unit Value
                    {
                        get => _value;
                        set => this.RaiseAndSetIfChanged(ref _value, value);
                    }

                    private Unit _value;
                }
            }
            """;

        internal const string IncorrectMultipleProperty =

            // lang=csharp
            """
            using System;
            using System.Reactive;
            using ReactiveUI;

            namespace Sample
            {
                public class WhenAnyValueClosureExample : ReactiveObject
                {
                    public WhenAnyValueClosureExample()
                    {
                        this.WhenAnyValue(x => Value, y => y.Value).Subscribe();
                        this.WhenAnyValue(x => x.Value, y => Value).Subscribe();
                        this.WhenAnyValue(x => Value, y => Value).Subscribe();
                    }

                    public Unit Value
                    {
                        get => _value;
                        set => this.RaiseAndSetIfChanged(ref _value, value);
                    }

                    private Unit _value;
                }
            }
            """;
    }
}