using FluentAssertions;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;
using Rocket.Surgery.Airframe.CodeFixes.Usage;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Usages;

public class Rsa1008FixTests
{
    [Theory]
    [ClassData(typeof(Rsa1008FixTestData))]
    public async Task GivenDirectAssignment_WhenVerified_ThenDiagnosticsResolved(string incorrect, int diagnosticCount)
    {
        // Given, When, Then
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1008>()
           .WithCodeFix<Rsa1008Fix>()
           .AddSources(incorrect)
           .Build()
           .GenerateAsync();

        result
           .CodeFixResults[typeof(Rsa1008Fix)]
           .ResolvedFixes
           .Should()
           .HaveCount(diagnosticCount)
           .And
           .Subject
           .Should()
           .Contain(testResult => testResult.Diagnostic.Descriptor == Descriptions.RSA1008);
    }

    [Theory]
    [InlineData(nameof(Rsa1008FixTestData.ArrowBodied), Rsa1008FixTestData.ArrowBodied)]
    [InlineData(nameof(Rsa1008FixTestData.BlockBodied), Rsa1008FixTestData.BlockBodied)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1008>()
           .WithCodeFix<Rsa1008Fix>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(
                typeof(Unit),
                typeof(ICommand),
                typeof(ReactiveCommand),
                typeof(Expression<>),
                typeof(CompositeDisposable))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }
}

public class Rsa1008FixTestData : IEnumerable<object[]>
{
    /// <inheritdoc/>
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            ArrowBodied,
            1
        ];

        yield return
        [
            BlockBodied,
            1
        ];
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal const string ArrowBodied = @"
using System;
using ReactiveUI;

namespace Sample
{
    public class DirectAssignmentArrowExample : ReactiveObject
    {
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        private string _name;
    }
}";

    internal const string BlockBodied = @"
using System;
using ReactiveUI;

namespace Sample
{
    public class DirectAssignmentBlockExample : ReactiveObject
    {
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _name;
    }
}";
}
