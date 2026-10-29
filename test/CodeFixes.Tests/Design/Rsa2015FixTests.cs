using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

public class Rsa2015FixTests
{
    [Theory]
    [InlineData(nameof(ImplicitInterfaceImplementation), ImplicitInterfaceImplementation)]
    [InlineData(nameof(ExplicitInterfaceImplementation), ExplicitInterfaceImplementation)]
    [InlineData(nameof(AbstractOverride), AbstractOverride)]
    [InlineData(nameof(OverrideWithAttribute), OverrideWithAttribute)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2015>()
           .WithCodeFix<Rsa2015Fix>()
           .AddNormalizedSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    // lang=csharp
    internal const string ImplicitInterfaceImplementation =
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
    internal const string ExplicitInterfaceImplementation =
        """
        namespace Sample
        {
            public interface IReader
            {
                string Read(string key);
            }

            public class Reader : IReader
            {
                string IReader.Read(string key) => key;
            }
        }
        """;

    // lang=csharp
    internal const string AbstractOverride =
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
    /// The tag has to land above the attribute list, not between it and the declaration, or the
    /// attribute would no longer sit directly on the member.
    /// </summary>
    // lang=csharp
    internal const string OverrideWithAttribute =
        """
        namespace Sample
        {
            public abstract class ReaderBase
            {
                public abstract string Read(string key);
            }

            public class Reader : ReaderBase
            {
                [System.Obsolete]
                public override string Read(string key) => key;
            }
        }
        """;
}
