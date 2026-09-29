using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Nullability;

public class Rsa0002Tests
{
    [Theory]
    [InlineData(NonNullableMethodReturn)]
    [InlineData(NonNullablePropertyReturn)]
    [InlineData(NullableStringReturn)]
    [InlineData(PrivateMethodNullableReturn)]
    [InlineData(InternalMethodNullableReturn)]
    [InlineData(PrivateProtectedMethodNullableReturn)]
    [InlineData(PublicMethodOnInternalNestedType)]
    [InlineData(PublicMethodOnPrivateNestedType)]
    [InlineData(PublicMethodOnFileScopedType)]
    [InlineData(NullableElementNonNullableCollectionReturn)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0002>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA0002.Id));
    }

    [Theory]
    [InlineData(OverrideNullableReturn)]
    [InlineData(ExplicitInterfaceImplementationNullableReturn)]
    public async Task GivenOverrideOrExplicitImplementation_WhenAnalyze_ThenOnlyOriginDeclarationReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0002>()
           .GenerateAsync();

        // Then. The origin abstract/interface member reports; the override and the explicit
        // implementation do not, since neither author chose that member's nullability.
        result
           .AnalyzerResults[typeof(Rsa0002)]
           .Diagnostics
           .Should()
           .ContainSingle(diagnostic => diagnostic.Id == RSA0002.Id);
    }

    [Theory]
    [InlineData(PublicMethodNullableEnumerableReturn)]
    [InlineData(PublicMethodNullableListReturn)]
    [InlineData(PublicMethodNullableArrayReturn)]
    [InlineData(ProtectedMethodNullableReturn)]
    [InlineData(PublicPropertyNullableReturn)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0002>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0002)]
           .Diagnostics
           .Should()
           .NotBeEmpty()
           .And
           .OnlyContain(diagnostic => diagnostic.Id == RSA0002.Id);
    }

    // lang=csharp
    internal const string NonNullableMethodReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Example
            {
                public IEnumerable<string> Read() => [];
            }
        }
        """;

    // lang=csharp
    internal const string NonNullablePropertyReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Example
            {
                public List<string> Items { get; } = [];
            }
        }
        """;

    // lang=csharp
    internal const string NullableStringReturn =
        """
        namespace Sample
        {
            public class Example
            {
                public string? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PrivateMethodNullableReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Example
            {
                private IEnumerable<string>? Read() => null;

                public void Use() => Read();
            }
        }
        """;

    // lang=csharp
    internal const string InternalMethodNullableReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            internal class Example
            {
                internal IEnumerable<string>? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PrivateProtectedMethodNullableReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Example
            {
                private protected IEnumerable<string>? Read() => null;

                public void Use() => Read();
            }
        }
        """;

    // lang=csharp
    internal const string PublicMethodOnInternalNestedType =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Outer
            {
                internal class Inner
                {
                    public IEnumerable<string>? Read() => null;
                }
            }
        }
        """;

    // lang=csharp
    internal const string PublicMethodOnPrivateNestedType =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Outer
            {
                private class Inner
                {
                    public IEnumerable<string>? Read() => null;
                }

                public void Use() => new Inner().Read();
            }
        }
        """;

    // lang=csharp
    internal const string PublicMethodOnFileScopedType =
        """
        using System.Collections.Generic;

        file class Example
        {
            public IEnumerable<string>? Read() => null;
        }
        """;

    // lang=csharp
    internal const string OverrideNullableReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public abstract class Base
            {
                public abstract IEnumerable<string>? Read();
            }

            public class Example : Base
            {
                public override IEnumerable<string>? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string ExplicitInterfaceImplementationNullableReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public interface IReader
            {
                IEnumerable<string>? Read();
            }

            public class Example : IReader
            {
                IEnumerable<string>? IReader.Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string NullableElementNonNullableCollectionReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Example
            {
                public IEnumerable<string?> Read() => [];
            }
        }
        """;

    // lang=csharp
    internal const string PublicMethodNullableEnumerableReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Example
            {
                public IEnumerable<string>? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PublicMethodNullableListReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Example
            {
                public List<string>? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PublicMethodNullableArrayReturn =
        """
        namespace Sample
        {
            public class Example
            {
                public string[]? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string ProtectedMethodNullableReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Example
            {
                protected IEnumerable<string>? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PublicPropertyNullableReturn =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public class Example
            {
                public IReadOnlyList<string>? Items { get; } = null;
            }
        }
        """;
}
