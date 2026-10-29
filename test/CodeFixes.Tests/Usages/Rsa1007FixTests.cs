using Microsoft.CodeAnalysis;
using ReactiveUI;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Usage;
using Rocket.Surgery.Airframe.CodeFixes.Usage;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Usages;

public class Rsa1007FixTests
{
    [Theory]
    [InlineData(nameof(Incorrect), Incorrect)]
    [InlineData(nameof(IncorrectWithParameter), IncorrectWithParameter)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa1007>()
           .WithCodeFix<Rsa1007Fix>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .AddReferences(
                typeof(Unit),
                typeof(ICommand),
                typeof(ReactiveCommand),
                typeof(BindingList<int>),
                typeof(Expression<>),
                typeof(CompositeDisposable))
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    internal const string Incorrect =

        // lang=csharp
        """
        using System.Threading;

        namespace Sample
        {
            public class FunctionInvocation
            {
                public FunctionInvocation()
                {
                    var func = () => string.Empty;
                        
                    var result = func();
                }
            }
        }
        """;
    internal const string IncorrectWithParameter =

        // lang=csharp
        """
        using System.Threading;

        namespace Sample
        {
            public class FunctionWithParametersInvocation
            {
                public FunctionWithParametersInvocation()
                {
                    var func = (string stuff) => string.Empty;
            
                    var result = func(string.Empty);
                }
            }
        }
        """;
}