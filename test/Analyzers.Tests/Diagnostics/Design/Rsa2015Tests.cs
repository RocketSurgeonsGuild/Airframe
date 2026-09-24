using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Design;

public class Rsa2015Tests
{
    [Theory]
    [InlineData(ImplicitInterfaceImplementationWithInheritdoc)]
    [InlineData(AbstractOverrideWithInheritdoc)]
    [InlineData(ExplicitInterfaceImplementationWithInheritdoc)]
    [InlineData(NonImplementingMember)]
    [InlineData(ReabstractedOverride)]
    [InlineData(InheritedImplementationOnDerivedType)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2015>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults
           .Should()
           .NotContain(pair => pair.Value.Diagnostics.Any(diagnostic => diagnostic.Id == RSA2015.Id));
    }

    [Theory]
    [InlineData(ImplicitInterfaceImplementationWithoutInheritdoc)]
    [InlineData(AbstractOverrideWithoutInheritdoc)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddSources(source)
           .WithAnalyzer<Rsa2015>()
           .GenerateAsync();

        // Then
        result
           .AnalyzerResults[typeof(Rsa2015)]
           .Diagnostics
           .Should()
           .ContainSingle()
           .Which
           .Id
           .Should()
           .Be(RSA2015.Id);
    }

    // lang=csharp
    internal const string ImplicitInterfaceImplementationWithInheritdoc =
        """
        namespace Sample
        {
            public interface IReader
            {
                string Read(string key);
            }

            public class Reader : IReader
            {
                /// <inheritdoc/>
                public string Read(string key) => key;
            }
        }
        """;

    // lang=csharp
    internal const string ImplicitInterfaceImplementationWithoutInheritdoc =
        """
        namespace Sample
        {
            public interface IReader
            {
                string Read(string key);
            }

            public class Reader : IReader
            {
                public string Read(string key) => key;
            }
        }
        """;

    // lang=csharp
    internal const string ExplicitInterfaceImplementationWithInheritdoc =
        """
        namespace Sample
        {
            public interface IReader
            {
                string Read(string key);
            }

            public class Reader : IReader
            {
                /// <inheritdoc/>
                string IReader.Read(string key) => key;
            }
        }
        """;

    // lang=csharp
    internal const string AbstractOverrideWithInheritdoc =
        """
        namespace Sample
        {
            public abstract class ReaderBase
            {
                public abstract string Read(string key);
            }

            public class Reader : ReaderBase
            {
                /// <inheritdoc/>
                public override string Read(string key) => key;
            }
        }
        """;

    // lang=csharp
    internal const string AbstractOverrideWithoutInheritdoc =
        """
        namespace Sample
        {
            public abstract class ReaderBase
            {
                public abstract string Read(string key);
            }

            public class Reader : ReaderBase
            {
                public override string Read(string key) => key;
            }
        }
        """;

    /// <summary>
    /// A member that neither implements an interface member nor overrides an abstract member.
    /// RSA2015 never applies to it, documented or not.
    /// </summary>
    // lang=csharp
    internal const string NonImplementingMember =
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
    /// An override that is itself abstract, re-declaring the contract rather than fulfilling it.
    /// RSA2014 owns documenting this one; RSA2015 leaves it alone.
    /// </summary>
    // lang=csharp
    internal const string ReabstractedOverride =
        """
        namespace Sample
        {
            public abstract class ReaderBase
            {
                public abstract string Read(string key);
            }

            /// <summary>A reader that still has no answer for <see cref="Read"/>.</summary>
            public abstract class ReaderMiddle : ReaderBase
            {
                /// <inheritdoc/>
                public abstract override string Read(string key);
            }
        }
        """;

    /// <summary>
    /// A type that inherits an interface implementation from its base class without overriding
    /// it. Only the type that actually writes the member out needs <c>&lt;inheritdoc/&gt;</c>.
    /// </summary>
    // lang=csharp
    internal const string InheritedImplementationOnDerivedType =
        """
        namespace Sample
        {
            public interface IReader
            {
                string Read(string key);
            }

            public class ReaderBase : IReader
            {
                /// <inheritdoc/>
                public string Read(string key) => key;
            }

            public class Reader : ReaderBase
            {
                public string Describe() => "reader";
            }
        }
        """;
}
