using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Nullability;

public class Rsa0004Tests
{
    [Theory]
    [InlineData(nameof(MethodWithNoNullForgiving), MethodWithNoNullForgiving)]
    [InlineData(nameof(MethodWithThreeNullForgiving), MethodWithThreeNullForgiving)]
    [InlineData(nameof(ConstructorWithThreeNullForgiving), ConstructorWithThreeNullForgiving)]
    [InlineData(nameof(PropertyWithThreeNullForgiving), PropertyWithThreeNullForgiving)]
    [InlineData(nameof(MethodWithFourNullForgivingBeforeIsChecks), MethodWithFourNullForgivingBeforeIsChecks)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0004>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA0004.Id), because: $"{name} should not report RSA0004");
    }

    [Theory]
    [InlineData(nameof(MethodWithFourNullForgiving), MethodWithFourNullForgiving)]
    [InlineData(nameof(ConstructorWithFourNullForgiving), ConstructorWithFourNullForgiving)]
    [InlineData(nameof(PropertyWithFourNullForgivingAcrossAccessors), PropertyWithFourNullForgivingAcrossAccessors)]
    [InlineData(nameof(ExpressionBodiedPropertyWithFourNullForgiving), ExpressionBodiedPropertyWithFourNullForgiving)]
    [InlineData(nameof(NullForgivingInsideNestedLocalFunctionCountsTowardOuterMethod), NullForgivingInsideNestedLocalFunctionCountsTowardOuterMethod)]
    [InlineData(nameof(IndexerWithFourNullForgiving), IndexerWithFourNullForgiving)]
    [InlineData(nameof(OperatorWithFourNullForgiving), OperatorWithFourNullForgiving)]
    [InlineData(nameof(ConversionOperatorWithFourNullForgiving), ConversionOperatorWithFourNullForgiving)]
    [InlineData(nameof(PropertyInitializerWithFourNullForgiving), PropertyInitializerWithFourNullForgiving)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0004>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0004)]
           .Diagnostics
           .Should()
           .ContainSingle(diagnostic => diagnostic.Id == RSA0004.Id, because: $"{name} should report RSA0004");
    }

    [Fact]
    public async Task GivenConfiguredThreshold_WhenAnalyze_ThenThresholdIsRespected()
    {
        // Given, When. Two uses, but the configured threshold is one.
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(MethodWithTwoNullForgiving)
           .WithAnalyzer<Rsa0004>()
           .AddGlobalOption("rsa0004_max_null_forgiving_operators", "1")
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0004)]
           .Diagnostics
           .Should()
           .ContainSingle(diagnostic => diagnostic.Id == RSA0004.Id, because: "the configured threshold of 1 is lower than the default of 3");
    }

    [Fact]
    public async Task GivenThresholdConfiguredToZero_WhenAnalyze_ThenAnySingleUseIsReported()
    {
        // Given, When. One use, but the configured threshold is zero - banning the operator
        // outright. "0" must not be silently treated as "not configured" and fall back to the
        // default of 3, which would leave a single use unreported.
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(MethodWithOneNullForgiving)
           .WithAnalyzer<Rsa0004>()
           .AddGlobalOption("rsa0004_max_null_forgiving_operators", "0")
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0004)]
           .Diagnostics
           .Should()
           .ContainSingle(diagnostic => diagnostic.Id == RSA0004.Id, because: "a configured threshold of 0 means any use at all should report");
    }

    [Fact]
    public async Task GivenThresholdConfiguredToZero_WhenAnalyzeWithNoUses_ThenNoDiagnosticsReported()
    {
        // Given, When. Zero uses and a configured threshold of zero: 0 is not > 0, so this must
        // not false-positive on a member that never uses the operator at all.
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(MethodWithNoNullForgiving)
           .WithAnalyzer<Rsa0004>()
           .AddGlobalOption("rsa0004_max_null_forgiving_operators", "0")
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA0004.Id));
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("banana")]
    [InlineData("")]
    public async Task GivenInvalidThreshold_WhenAnalyze_ThenDefaultThresholdIsUsed(string invalidValue)
    {
        // Given, When. Three uses is at the default threshold of 3 (not > 3), so an invalid
        // configured value - negative, non-numeric, or empty - must fall back to the default
        // rather than be treated as "any use reports" the way 0 legitimately is.
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(MethodWithThreeNullForgiving)
           .WithAnalyzer<Rsa0004>()
           .AddGlobalOption("rsa0004_max_null_forgiving_operators", invalidValue)
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA0004.Id), because: $"'{invalidValue}' is not a valid threshold and should fall back to the default of 3");
    }

    // lang=csharp
    internal const string MethodWithOneNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string Read(string? a) => a!;
            }
        }
        """;

    // lang=csharp
    internal const string MethodWithNoNullForgiving =
        """
        namespace Sample
        {
            public class Example
            {
                public string Read(string? value) => value ?? string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string MethodWithThreeNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string Read(string? a, string? b, string? c) => a! + b! + c!;
            }
        }
        """;

    // lang=csharp
    internal const string ConstructorWithThreeNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                private readonly string _a;
                private readonly string _b;
                private readonly string _c;

                public Example(string? a, string? b, string? c)
                {
                    _a = a!;
                    _b = b!;
                    _c = c!;
                }
            }
        }
        """;

    // lang=csharp
    internal const string PropertyWithThreeNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                private string? _a;
                private string? _b;
                private string? _c;

                public string Value
                {
                    get => _a! + _b!;
                    set => _c = value!;
                }
            }
        }
        """;

    // lang=csharp
    internal const string MethodWithFourNullForgivingBeforeIsChecks =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public bool Check(object? a, object? b, object? c, object? d) =>
                    a! is string &&
                    b! is string s &&
                    c! is not null &&
                    d! is int;
            }
        }
        """;

    // lang=csharp
    internal const string MethodWithFourNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string Read(string? a, string? b, string? c, string? d) => a! + b! + c! + d!;
            }
        }
        """;

    // lang=csharp
    internal const string IndexerWithFourNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                private string? _a, _b, _c, _d;

                public string this[int index] => _a! + _b! + _c! + _d!;
            }
        }
        """;

    // lang=csharp
    internal const string OperatorWithFourNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                private string? _a, _b, _c, _d;

                public static string operator +(Example left, Example right) => left._a! + left._b! + left._c! + left._d!;
            }
        }
        """;

    // lang=csharp
    internal const string ConversionOperatorWithFourNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                private string? _a, _b, _c, _d;

                public static implicit operator string(Example value) => value._a! + value._b! + value._c! + value._d!;
            }
        }
        """;

    // lang=csharp
    internal const string PropertyInitializerWithFourNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                private static readonly string? _a, _b, _c, _d;

                public string Combined { get; set; } = _a! + _b! + _c! + _d!;
            }
        }
        """;

    // lang=csharp
    internal const string ConstructorWithFourNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                private readonly string _a;
                private readonly string _b;
                private readonly string _c;
                private readonly string _d;

                public Example(string? a, string? b, string? c, string? d)
                {
                    _a = a!;
                    _b = b!;
                    _c = c!;
                    _d = d!;
                }
            }
        }
        """;

    // lang=csharp
    internal const string PropertyWithFourNullForgivingAcrossAccessors =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                private string? _a;
                private string? _b;
                private string? _c;
                private string? _d;

                public string Value
                {
                    get => _a! + _b!;
                    set
                    {
                        _c = value!;
                        _d = value!;
                    }
                }
            }
        }
        """;

    // lang=csharp
    internal const string ExpressionBodiedPropertyWithFourNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                private string? _a;
                private string? _b;
                private string? _c;
                private string? _d;

                public string Value => _a! + _b! + _c! + _d!;
            }
        }
        """;

    // lang=csharp
    internal const string NullForgivingInsideNestedLocalFunctionCountsTowardOuterMethod =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string Read(string? a, string? b, string? c, string? d)
                {
                    return Combine();

                    string Combine() => a! + b! + c! + d!;
                }
            }
        }
        """;

    // lang=csharp
    internal const string MethodWithTwoNullForgiving =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string Read(string? a, string? b) => a! + b!;
            }
        }
        """;
}
