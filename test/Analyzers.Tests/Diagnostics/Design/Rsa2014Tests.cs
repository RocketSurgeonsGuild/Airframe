using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Design;

public class Rsa2014Tests
{
    [Theory]
    [InlineData(DocumentedInterface)]
    [InlineData(DocumentedAbstractClass)]
    [InlineData(ConcreteClassWithoutDocs)]
    [InlineData(DefaultInterfaceMember)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2014>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA2014.Id));
    }

    [Theory]
    [InlineData(UndocumentedInterface)]
    [InlineData(UndocumentedAbstractClass)]
    [InlineData(UndocumentedAbstractMember)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2014>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa2014)]
           .Diagnostics
           .Should()
           .NotBeEmpty()
           .And
           .OnlyContain(diagnostic => diagnostic.Id == RSA2014.Id);
    }

    // lang=csharp
    internal const string DocumentedInterface =
        """
        namespace Sample
        {
            /// <summary>Reads a value by key.</summary>
            public interface IReader
            {
                /// <summary>Gets the value for <paramref name="key"/>.</summary>
                string Read(string key);
            }
        }
        """;

    // lang=csharp
    internal const string UndocumentedInterface =
        """
        namespace Sample
        {
            public interface IReader
            {
                /// <summary>Gets the value for <paramref name="key"/>.</summary>
                string Read(string key);
            }
        }
        """;

    // lang=csharp
    internal const string DocumentedAbstractClass =
        """
        namespace Sample
        {
            /// <summary>Reads a value by key.</summary>
            public abstract class Reader
            {
                /// <summary>Gets the value for <paramref name="key"/>.</summary>
                public abstract string Read(string key);

                // Concrete members are not part of the contract; no doc required.
                public string Describe() => "reader";
            }
        }
        """;

    // lang=csharp
    internal const string UndocumentedAbstractClass =
        """
        namespace Sample
        {
            public abstract class Reader
            {
                /// <summary>Gets the value for <paramref name="key"/>.</summary>
                public abstract string Read(string key);
            }
        }
        """;

    // lang=csharp
    internal const string UndocumentedAbstractMember =
        """
        namespace Sample
        {
            /// <summary>Reads a value by key.</summary>
            public abstract class Reader
            {
                public abstract string Read(string key);
            }
        }
        """;

    /// <summary>
    /// A concrete class with no XML documentation at all. RSA2014 never applies outside an
    /// interface or an abstract type, so a project that skips documentation entirely on its
    /// concrete types stays silent under this rule.
    /// </summary>
    // lang=csharp
    internal const string ConcreteClassWithoutDocs =
        """
        namespace Sample
        {
            public class Reader
            {
                public string Read(string key) => key;
            }
        }
        """;

    /// <summary>
    /// A default interface member carries its own body, so it is not part of the contract an
    /// implementer has to fulfil sight unseen and is excluded even though it is undocumented.
    /// </summary>
    // lang=csharp
    internal const string DefaultInterfaceMember =
        """
        namespace Sample
        {
            /// <summary>Reads a value by key.</summary>
            public interface IReader
            {
                string Describe() => "reader";
            }
        }
        """;
}
