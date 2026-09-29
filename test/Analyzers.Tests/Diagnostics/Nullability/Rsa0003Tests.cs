using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Nullability;

public class Rsa0003Tests
{
    [Theory]
    [InlineData(NonNullableTaskReturn)]
    [InlineData(NonNullableTaskOfTReturn)]
    [InlineData(NonNullableValueTaskReturn)]
    [InlineData(NullableResultTaskReturn)]
    [InlineData(PrivateMethodNullableTaskReturn)]
    [InlineData(InternalMethodNullableTaskReturn)]
    [InlineData(PrivateProtectedMethodNullableTaskReturn)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0003>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA0003.Id));
    }

    [Theory]
    [InlineData(OverrideNullableTaskReturn)]
    [InlineData(ExplicitInterfaceImplementationNullableTaskReturn)]
    public async Task GivenOverrideOrExplicitImplementation_WhenAnalyze_ThenOnlyOriginDeclarationReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0003>()
           .GenerateAsync();

        // Then. The origin abstract/interface member reports; the override and the explicit
        // implementation do not, since neither author chose that member's nullability.
        result
           .AnalyzerResults[typeof(Rsa0003)]
           .Diagnostics
           .Should()
           .ContainSingle(diagnostic => diagnostic.Id == RSA0003.Id);
    }

    [Theory]
    [InlineData(nameof(PublicMethodNullableTaskReturn), PublicMethodNullableTaskReturn)]
    [InlineData(nameof(PublicMethodNullableTaskOfTReturn), PublicMethodNullableTaskOfTReturn)]
    [InlineData(nameof(PublicMethodNullableValueTaskReturn), PublicMethodNullableValueTaskReturn)]
    [InlineData(nameof(ProtectedMethodNullableTaskReturn), ProtectedMethodNullableTaskReturn)]
    [InlineData(nameof(PublicPropertyNullableTaskReturn), PublicPropertyNullableTaskReturn)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa0003>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa0003)]
           .Diagnostics
           .Should()
           .NotBeEmpty(because: $"{name} should report RSA0003")
           .And
           .OnlyContain(diagnostic => diagnostic.Id == RSA0003.Id);
    }

    // lang=csharp
    internal const string NonNullableTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                public Task Read() => Task.CompletedTask;
            }
        }
        """;

    // lang=csharp
    internal const string NonNullableTaskOfTReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                public Task<string> Read() => Task.FromResult(string.Empty);
            }
        }
        """;

    // lang=csharp
    internal const string NonNullableValueTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                public ValueTask Read() => default;
            }
        }
        """;

    // lang=csharp
    internal const string NullableResultTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                public Task<string?> Read() => Task.FromResult<string?>(null);
            }
        }
        """;

    // lang=csharp
    internal const string PrivateMethodNullableTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                private Task<string>? Read() => null;

                public void Use() => Read();
            }
        }
        """;

    // lang=csharp
    internal const string InternalMethodNullableTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            internal class Example
            {
                internal Task<string>? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PrivateProtectedMethodNullableTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                private protected Task<string>? Read() => null;

                public void Use() => Read();
            }
        }
        """;

    // lang=csharp
    internal const string OverrideNullableTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public abstract class Base
            {
                public abstract Task<string>? Read();
            }

            public class Example : Base
            {
                public override Task<string>? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string ExplicitInterfaceImplementationNullableTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public interface IReader
            {
                Task<string>? Read();
            }

            public class Example : IReader
            {
                Task<string>? IReader.Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PublicMethodNullableTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                public Task? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PublicMethodNullableTaskOfTReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                public Task<string>? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PublicMethodNullableValueTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                public ValueTask<string>? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string ProtectedMethodNullableTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                protected Task<string>? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PublicPropertyNullableTaskReturn =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public class Example
            {
                public Task<string>? Value { get; }
            }
        }
        """;
}
