using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Nullability;

public class Rsa0006Tests
{
    [Theory]
    [InlineData(nameof(NonNullableBooleanProperty), NonNullableBooleanProperty)]
    [InlineData(nameof(PrivateNullableBooleanProperty), PrivateNullableBooleanProperty)]
    [InlineData(nameof(InternalNullableBooleanProperty), InternalNullableBooleanProperty)]
    [InlineData(nameof(PrivateProtectedNullableBooleanProperty), PrivateProtectedNullableBooleanProperty)]
    [InlineData(nameof(PublicPropertyOnInternalNestedType), PublicPropertyOnInternalNestedType)]
    [InlineData(nameof(PublicPropertyOnPrivateNestedType), PublicPropertyOnPrivateNestedType)]
    [InlineData(nameof(PublicPropertyOnFileScopedType), PublicPropertyOnFileScopedType)]
    [InlineData(nameof(NullableIntProperty), NullableIntProperty)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0006>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA0006.Id), because: $"{name} should not report RSA0006");
    }

    [Theory]
    [InlineData(OverrideNullableBooleanProperty)]
    [InlineData(ExplicitInterfaceImplementationNullableBooleanProperty)]
    public async Task GivenOverrideOrExplicitImplementation_WhenAnalyze_ThenOnlyOriginDeclarationReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0006>()
           .GenerateAsync();

        // Then. The origin abstract/interface member reports; the override and the explicit
        // implementation do not, since neither author chose that member's type.
        result
           .AnalyzerResults[typeof(Rsa0006)]
           .Diagnostics
           .Should()
           .ContainSingle(diagnostic => diagnostic.Id == RSA0006.Id);
    }

    [Theory]
    [InlineData(nameof(PublicNullableBooleanProperty), PublicNullableBooleanProperty)]
    [InlineData(nameof(ProtectedNullableBooleanProperty), ProtectedNullableBooleanProperty)]
    [InlineData(nameof(PublicNullableBooleanIndexer), PublicNullableBooleanIndexer)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0006>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0006)]
           .Diagnostics
           .Should()
           .NotBeEmpty(because: $"{name} should report RSA0006")
           .And
           .OnlyContain(diagnostic => diagnostic.Id == RSA0006.Id);
    }

    // lang=csharp
    internal const string NonNullableBooleanProperty =
        """
        namespace Sample
        {
            public class Example
            {
                public bool Flag { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string PrivateNullableBooleanProperty =
        """
        namespace Sample
        {
            public class Example
            {
                private bool? Flag { get; set; }

                public bool? Use() => Flag;
            }
        }
        """;

    // lang=csharp
    internal const string InternalNullableBooleanProperty =
        """
        namespace Sample
        {
            internal class Example
            {
                internal bool? Flag { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string PrivateProtectedNullableBooleanProperty =
        """
        namespace Sample
        {
            public class Example
            {
                private protected bool? Flag { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string PublicPropertyOnInternalNestedType =
        """
        namespace Sample
        {
            public class Outer
            {
                internal class Inner
                {
                    public bool? Flag { get; set; }
                }
            }
        }
        """;

    // lang=csharp
    internal const string PublicPropertyOnPrivateNestedType =
        """
        namespace Sample
        {
            public class Outer
            {
                private class Inner
                {
                    public bool? Flag { get; set; }
                }

                public void Use() => new Inner();
            }
        }
        """;

    // lang=csharp
    internal const string PublicPropertyOnFileScopedType =
        """
        file class Example
        {
            public bool? Flag { get; set; }
        }
        """;

    // lang=csharp
    internal const string NullableIntProperty =
        """
        namespace Sample
        {
            public class Example
            {
                public int? Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string OverrideNullableBooleanProperty =
        """
        namespace Sample
        {
            public abstract class Base
            {
                public abstract bool? Flag { get; }
            }

            public class Example : Base
            {
                public override bool? Flag { get; }
            }
        }
        """;

    // lang=csharp
    internal const string ExplicitInterfaceImplementationNullableBooleanProperty =
        """
        namespace Sample
        {
            public interface IHasFlag
            {
                bool? Flag { get; }
            }

            public class Example : IHasFlag
            {
                bool? IHasFlag.Flag => null;
            }
        }
        """;

    // lang=csharp
    internal const string PublicNullableBooleanProperty =
        """
        namespace Sample
        {
            public class Example
            {
                public bool? Flag { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string ProtectedNullableBooleanProperty =
        """
        namespace Sample
        {
            public class Example
            {
                protected bool? Flag { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string PublicNullableBooleanIndexer =
        """
        namespace Sample
        {
            public class Example
            {
                public bool? this[int index] => null;
            }
        }
        """;
}
