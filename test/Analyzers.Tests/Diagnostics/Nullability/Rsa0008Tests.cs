using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Nullability;

public class Rsa0008Tests
{
    [Theory]
    [InlineData(nameof(NoDirectiveAtAll), NoDirectiveAtAll)]
    [InlineData(nameof(NullableEnable), NullableEnable)]
    [InlineData(nameof(NullableEnableAnnotationsOnly), NullableEnableAnnotationsOnly)]
    [InlineData(nameof(NullableDisableWarningsOnly), NullableDisableWarningsOnly)]
    [InlineData(nameof(NullableRestoreWarningsOnly), NullableRestoreWarningsOnly)]
    [InlineData(nameof(BareNullableDirectiveMissingSetting), BareNullableDirectiveMissingSetting)]
    [InlineData(nameof(DisableInsideInactiveIfBlock), DisableInsideInactiveIfBlock)]
    [InlineData(nameof(DisableInsideNotTakenElseBlock), DisableInsideNotTakenElseBlock)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0008>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA0008.Id), because: $"{name} should not report RSA0008");
    }

    [Theory]
    [InlineData(nameof(NullableDisable), NullableDisable)]
    [InlineData(nameof(NullableRestore), NullableRestore)]
    [InlineData(nameof(NullableDisableAnnotations), NullableDisableAnnotations)]
    [InlineData(nameof(NullableRestoreAnnotations), NullableRestoreAnnotations)]
    [InlineData(nameof(MultipleDisableAndRestoreDirectives), MultipleDisableAndRestoreDirectives)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0008>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0008)]
           .Diagnostics
           .Should()
           .NotBeEmpty(because: $"{name} should report RSA0008")
           .And
           .OnlyContain(diagnostic => diagnostic.Id == RSA0008.Id);
    }

    [Fact]
    public async Task GivenMultipleDirectives_WhenAnalyze_ThenEachIsReportedSeparately()
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(MultipleDisableAndRestoreDirectives)
           .WithAnalyzer<Rsa0008>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0008)]
           .Diagnostics
           .Where(diagnostic => diagnostic.Id == RSA0008.Id)
           .Should()
           .HaveCount(2);
    }

    // lang=csharp
    internal const string NoDirectiveAtAll =
        """
        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string NullableEnable =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string NullableEnableAnnotationsOnly =
        """
        #nullable enable annotations

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string NullableDisableWarningsOnly =
        """
        #nullable enable
        #nullable disable warnings

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string NullableRestoreWarningsOnly =
        """
        #nullable enable
        #nullable restore warnings

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string BareNullableDirectiveMissingSetting =
        """
        #nullable enable
        #nullable

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string DisableInsideInactiveIfBlock =
        """
        #nullable enable

        #if RSA0008_SYMBOL_NEVER_DEFINED
        #nullable disable
        #endif

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string DisableInsideNotTakenElseBlock =
        """
        #nullable enable

        #if true
        #else
        #nullable disable
        #endif

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string NullableDisable =
        """
        #nullable enable
        #nullable disable

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string NullableRestore =
        """
        #nullable enable
        #nullable restore

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string NullableDisableAnnotations =
        """
        #nullable enable
        #nullable disable annotations

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string NullableRestoreAnnotations =
        """
        #nullable enable
        #nullable restore annotations

        namespace Sample
        {
            public class Example
            {
                public string? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string MultipleDisableAndRestoreDirectives =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
        #nullable disable
                public string? Value { get; set; }
        #nullable restore
            }
        }
        """;
}
