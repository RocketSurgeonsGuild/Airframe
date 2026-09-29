using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Nullability;

public class Rsa0005Tests
{
    [Theory]
    [InlineData(nameof(GenuinelyMaybeNullConditionalAccess), GenuinelyMaybeNullConditionalAccess)]
    [InlineData(nameof(GenuinelyMaybeNullCoalesce), GenuinelyMaybeNullCoalesce)]
    [InlineData(nameof(GenuinelyMaybeNullCoalesceAssignment), GenuinelyMaybeNullCoalesceAssignment)]
    [InlineData(nameof(NonNullableDeclaredTypeConditionalAccess), NonNullableDeclaredTypeConditionalAccess)]
    [InlineData(nameof(ObliviousContextConditionalAccess), ObliviousContextConditionalAccess)]
    [InlineData(nameof(NarrowedThenReassignedMaybeNullBeforeUse), NarrowedThenReassignedMaybeNullBeforeUse)]
    [InlineData(nameof(NullableValueTypeCoalesceAfterLiteralAssignment), NullableValueTypeCoalesceAfterLiteralAssignment)]
    [InlineData(nameof(NullableValueTypeConditionalAccessAfterNullCheck), NullableValueTypeConditionalAccessAfterNullCheck)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0005>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA0005.Id), because: $"{name} should not report RSA0005");
    }

    [Theory]
    [InlineData(nameof(NarrowedByNullCheckConditionalAccess), NarrowedByNullCheckConditionalAccess)]
    [InlineData(nameof(NarrowedByLiteralAssignmentConditionalAccess), NarrowedByLiteralAssignmentConditionalAccess)]
    [InlineData(nameof(NarrowedByLiteralAssignmentCoalesce), NarrowedByLiteralAssignmentCoalesce)]
    [InlineData(nameof(NarrowedByLiteralAssignmentCoalesceAssignment), NarrowedByLiteralAssignmentCoalesceAssignment)]
    [InlineData(nameof(NarrowedByPatternMatchConditionalAccess), NarrowedByPatternMatchConditionalAccess)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0005>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0005)]
           .Diagnostics
           .Should()
           .NotBeEmpty(because: $"{name} should report RSA0005")
           .And
           .OnlyContain(diagnostic => diagnostic.Id == RSA0005.Id);
    }

    // lang=csharp
    internal const string GenuinelyMaybeNullConditionalAccess =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public int? Read(string? value) => value?.Length;
            }
        }
        """;

    // lang=csharp
    internal const string GenuinelyMaybeNullCoalesce =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string Read(string? value) => value ?? "default";
            }
        }
        """;

    // lang=csharp
    internal const string GenuinelyMaybeNullCoalesceAssignment =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string Read(string? value)
                {
                    value ??= "default";
                    return value;
                }
            }
        }
        """;

    // lang=csharp
    internal const string NonNullableDeclaredTypeConditionalAccess =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public int? Read(string value) => value?.Length;
            }
        }
        """;

    // lang=csharp
    internal const string ObliviousContextConditionalAccess =
        """
        #nullable disable

        namespace Sample
        {
            public class Example
            {
                public int? Read(string value) => value?.Length;
            }
        }
        """;

    // lang=csharp
    internal const string NarrowedThenReassignedMaybeNullBeforeUse =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public int? Read(string? value, string? other)
                {
                    if (value != null)
                    {
                        value = other;
                    }

                    return value?.Length;
                }
            }
        }
        """;

    // lang=csharp
    internal const string NullableValueTypeCoalesceAfterLiteralAssignment =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public int Read()
                {
                    int? value = 1;
                    return value ?? 0;
                }
            }
        }
        """;

    // lang=csharp
    internal const string NullableValueTypeConditionalAccessAfterNullCheck =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string? Read(int? value)
                {
                    if (value.HasValue)
                    {
                        return value?.ToString();
                    }

                    return null;
                }
            }
        }
        """;

    // lang=csharp
    internal const string NarrowedByNullCheckConditionalAccess =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public int? Read(string? value)
                {
                    if (value != null)
                    {
                        return value?.Length;
                    }

                    return null;
                }
            }
        }
        """;

    // lang=csharp
    internal const string NarrowedByLiteralAssignmentConditionalAccess =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public int? Read()
                {
                    string? value = "literal";
                    return value?.Length;
                }
            }
        }
        """;

    // lang=csharp
    internal const string NarrowedByLiteralAssignmentCoalesce =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string Read()
                {
                    string? value = "literal";
                    return value ?? "default";
                }
            }
        }
        """;

    // lang=csharp
    internal const string NarrowedByLiteralAssignmentCoalesceAssignment =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public string Read()
                {
                    string? value = "literal";
                    value ??= "default";
                    return value;
                }
            }
        }
        """;

    // lang=csharp
    internal const string NarrowedByPatternMatchConditionalAccess =
        """
        #nullable enable

        namespace Sample
        {
            public class Example
            {
                public int? Read(string? value)
                {
                    if (value is not null)
                    {
                        return value?.Length;
                    }

                    return null;
                }
            }
        }
        """;
}
