using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Nullability;

public class Rsa0001Tests
{
    [Theory]
    [InlineData(nameof(NonNullableAutoPropertyWithInit), NonNullableAutoPropertyWithInit)]
    [InlineData(nameof(NullableAutoPropertyWithSet), NullableAutoPropertyWithSet)]
    [InlineData(nameof(GetOnlyProperty), GetOnlyProperty)]
    [InlineData(nameof(PrivateSetAutoProperty), PrivateSetAutoProperty)]
    [InlineData(nameof(InternalSetAutoProperty), InternalSetAutoProperty)]
    [InlineData(nameof(PrivateProtectedSetAutoProperty), PrivateProtectedSetAutoProperty)]
    [InlineData(nameof(PublicSetterOnInternalNestedType), PublicSetterOnInternalNestedType)]
    [InlineData(nameof(PublicSetterOnPrivateNestedType), PublicSetterOnPrivateNestedType)]
    [InlineData(nameof(PublicSetterOnFileScopedType), PublicSetterOnFileScopedType)]
    [InlineData(nameof(HandWrittenSetterAccessor), HandWrittenSetterAccessor)]
    [InlineData(nameof(ValueTypeAutoProperty), ValueTypeAutoProperty)]
    [InlineData(nameof(ReactiveAttributeProperty), ReactiveAttributeProperty)]
    [InlineData(nameof(NotifyPropertyChangedTypeProperty), NotifyPropertyChangedTypeProperty)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0001>()
           .AddReferences(typeof(System.ComponentModel.INotifyPropertyChanged))
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA0001.Id), because: $"{name} should not report RSA0001");
    }

    [Theory]
    [InlineData(OverrideNonNullableAutoProperty)]
    [InlineData(ExplicitInterfaceImplementationNonNullableAutoProperty)]
    public async Task GivenOverrideOrExplicitImplementation_WhenAnalyze_ThenOnlyOriginDeclarationReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0001>()
           .GenerateAsync();

        // Then. The origin abstract/interface member reports; the override and the explicit
        // implementation do not, since neither author chose that member's mutability.
        result
           .AnalyzerResults[typeof(Rsa0001)]
           .Diagnostics
           .Should()
           .ContainSingle(diagnostic => diagnostic.Id == RSA0001.Id);
    }

    [Theory]
    [InlineData(nameof(PublicNonNullableAutoPropertyWithSet), PublicNonNullableAutoPropertyWithSet)]
    [InlineData(nameof(ProtectedNonNullableAutoPropertyWithSet), ProtectedNonNullableAutoPropertyWithSet)]
    [InlineData(nameof(RequiredNonNullableAutoPropertyWithSet), RequiredNonNullableAutoPropertyWithSet)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0001>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0001)]
           .Diagnostics
           .Should()
           .ContainSingle(diagnostic => diagnostic.Id == RSA0001.Id, because: $"{name} should report RSA0001");
    }

    // lang=csharp
    internal const string NonNullableAutoPropertyWithInit =
        """
        namespace Sample
        {
            public class Example
            {
                public string Value { get; init; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string NullableAutoPropertyWithSet =
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
    internal const string GetOnlyProperty =
        """
        namespace Sample
        {
            public class Example
            {
                public string Value { get; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string PrivateSetAutoProperty =
        """
        namespace Sample
        {
            public class Example
            {
                public string Value { get; private set; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string InternalSetAutoProperty =
        """
        namespace Sample
        {
            public class Example
            {
                public string Value { get; internal set; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string PrivateProtectedSetAutoProperty =
        """
        namespace Sample
        {
            public class Example
            {
                public string Value { get; private protected set; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string PublicSetterOnInternalNestedType =
        """
        namespace Sample
        {
            public class Outer
            {
                internal class Inner
                {
                    public string Value { get; set; } = string.Empty;
                }
            }
        }
        """;

    // lang=csharp
    internal const string PublicSetterOnPrivateNestedType =
        """
        namespace Sample
        {
            public class Outer
            {
                private class Inner
                {
                    public string Value { get; set; } = string.Empty;
                }

                public void Use() => new Inner();
            }
        }
        """;

    // lang=csharp
    internal const string PublicSetterOnFileScopedType =
        """
        namespace Sample
        {
            file class Example
            {
                public string Value { get; set; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string HandWrittenSetterAccessor =
        """
        namespace Sample
        {
            public class Example
            {
                private string _value = string.Empty;

                public string Value
                {
                    get => _value;
                    set => _value = value ?? string.Empty;
                }
            }
        }
        """;

    // lang=csharp
    internal const string ValueTypeAutoProperty =
        """
        namespace Sample
        {
            public class Example
            {
                public int Value { get; set; }
            }
        }
        """;

    // lang=csharp
    internal const string ReactiveAttributeProperty =
        """
        namespace Sample
        {
            public class ReactiveAttribute : System.Attribute
            {
            }

            public class Example
            {
                [Reactive]
                public string Value { get; set; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string NotifyPropertyChangedTypeProperty =
        """
        using System.ComponentModel;

        namespace Sample
        {
            public class Example : INotifyPropertyChanged
            {
                public string Value { get; set; } = string.Empty;

                public event PropertyChangedEventHandler? PropertyChanged;
            }
        }
        """;

    // lang=csharp
    internal const string OverrideNonNullableAutoProperty =
        """
        namespace Sample
        {
            public abstract class Base
            {
                public abstract string Value { get; set; }
            }

            public class Example : Base
            {
                public override string Value { get; set; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string ExplicitInterfaceImplementationNonNullableAutoProperty =
        """
        namespace Sample
        {
            public interface IHasValue
            {
                string Value { get; set; }
            }

            public class Example : IHasValue
            {
                string IHasValue.Value { get; set; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string PublicNonNullableAutoPropertyWithSet =
        """
        namespace Sample
        {
            public class Example
            {
                public string Value { get; set; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string ProtectedNonNullableAutoPropertyWithSet =
        """
        namespace Sample
        {
            public class Example
            {
                protected string Value { get; set; } = string.Empty;
            }
        }
        """;

    // lang=csharp
    internal const string RequiredNonNullableAutoPropertyWithSet =
        """
        namespace Sample
        {
            public class Example
            {
                public required string Value { get; set; }
            }
        }
        """;
}
