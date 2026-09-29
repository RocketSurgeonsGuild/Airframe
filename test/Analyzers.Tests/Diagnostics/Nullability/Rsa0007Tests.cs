using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Nullability;

public class Rsa0007Tests
{
    [Theory]
    [InlineData(nameof(DefaultForValueType), DefaultForValueType)]
    [InlineData(nameof(DefaultSuppressedForNonNullableReferenceType), DefaultSuppressedForNonNullableReferenceType)]
    [InlineData(nameof(DefaultForUnconstrainedGeneric), DefaultForUnconstrainedGeneric)]
    [InlineData(nameof(NullAlreadyUsed), NullAlreadyUsed)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0007>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA0007.Id), because: $"{name} should not report RSA0007");
    }

    [Theory]
    [InlineData(nameof(DefaultLiteralFieldInitializer), DefaultLiteralFieldInitializer)]
    [InlineData(nameof(DefaultExpressionFieldInitializer), DefaultExpressionFieldInitializer)]
    [InlineData(nameof(DefaultParameterDefault), DefaultParameterDefault)]
    [InlineData(nameof(DefaultMethodReturn), DefaultMethodReturn)]
    [InlineData(nameof(DefaultLocalDeclaration), DefaultLocalDeclaration)]
    [InlineData(nameof(DefaultConstrainedGenericClass), DefaultConstrainedGenericClass)]
    [InlineData(nameof(DefaultArgumentAtCallSite), DefaultArgumentAtCallSite)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0007>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0007)]
           .Diagnostics
           .Should()
           .NotBeEmpty(because: $"{name} should report RSA0007")
           .And
           .OnlyContain(diagnostic => diagnostic.Id == RSA0007.Id);
    }

    // lang=csharp
    internal const string DefaultForValueType =
        """
        namespace Sample
        {
            public class Example
            {
                public int Value = default;
            }
        }
        """;

    // lang=csharp
    internal const string DefaultSuppressedForNonNullableReferenceType =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string Value = default!;
            }
        }
        """;

    // lang=csharp
    internal const string DefaultForUnconstrainedGeneric =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public T Get<T>() => default;
            }
        }
        """;

    // lang=csharp
    internal const string NullAlreadyUsed =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string? Value = null;
            }
        }
        """;

    // lang=csharp
    internal const string DefaultLiteralFieldInitializer =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string? Value = default;
            }
        }
        """;

    // lang=csharp
    internal const string DefaultExpressionFieldInitializer =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string? Value = default(string);
            }
        }
        """;

    // lang=csharp
    internal const string DefaultParameterDefault =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public void Read(string? value = default)
                {
                }
            }
        }
        """;

    // lang=csharp
    internal const string DefaultMethodReturn =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string? Read() => default;
            }
        }
        """;

    // lang=csharp
    internal const string DefaultLocalDeclaration =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public void Read()
                {
                    string? value = default;
                }
            }
        }
        """;

    // lang=csharp
    internal const string DefaultConstrainedGenericClass =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public T? Get<T>()
                    where T : class => default;
            }
        }
        """;

    // lang=csharp
    internal const string DefaultArgumentAtCallSite =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public void Read(string? value)
                {
                }

                public void Use() => Read(default);
            }
        }
        """;
}
