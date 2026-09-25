using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Design;

public class Rsa2014Tests
{
    [Theory]
    [InlineData(DocumentedInterface)]
    [InlineData(DocumentedAbstractClass)]
    [InlineData(ConcreteClassWithoutDocs)]
    [InlineData(DefaultInterfaceMember)]
    [InlineData(StaticDefaultInterfaceMember)]
    [InlineData(DocumentedStaticAbstractInterfaceMember)]
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
    [InlineData(UndocumentedStaticAbstractInterfaceMember)]
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

    /// <summary>
    /// A <c>static</c> default interface member. What excludes a default member is having a body,
    /// not being <c>static</c>, so this one is excluded the same as an instance default member.
    /// </summary>
    // lang=csharp
    internal const string StaticDefaultInterfaceMember =
        """
        namespace Sample
        {
            /// <summary>Reads a value by key.</summary>
            public interface IReader
            {
                static string Describe() => "reader";
            }
        }
        """;

    /// <summary>
    /// A <c>static abstract</c> interface member (the generic-math pattern): no body, so it is
    /// still part of the contract an implementer has to fulfil, and still needs a summary.
    /// </summary>
    // lang=csharp
    internal const string DocumentedStaticAbstractInterfaceMember =
        """
        namespace Sample
        {
            /// <summary>Parses a value.</summary>
            public interface IParsable
            {
                /// <summary>Parses <paramref name="value"/>.</summary>
                static abstract IParsable Parse(string value);
            }
        }
        """;

    // lang=csharp
    internal const string UndocumentedStaticAbstractInterfaceMember =
        """
        namespace Sample
        {
            /// <summary>Parses a value.</summary>
            public interface IParsable
            {
                static abstract IParsable Parse(string value);
            }
        }
        """;
}
