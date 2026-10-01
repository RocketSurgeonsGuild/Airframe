using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Nullability;

/// <remarks>
/// RSA0009 ships <c>isEnabledByDefault: false</c>, so the shared
/// <c>GeneratorTestContextBuilder</c> harness - which reports through Roslyn's normal
/// <c>CompilationWithAnalyzers</c> severity filtering - never surfaces it: a disabled descriptor
/// is filtered before it reaches the results, regardless of whether the analyzer's own logic would
/// have reported. These tests instead build a compilation directly and set
/// <c>CompilationOptions.SpecificDiagnosticOptions["RSA0009"] = Info</c>, the same override an
/// <c>.editorconfig</c> <c>dotnet_diagnostic.RSA0009.severity</c> entry produces, so the analyzer's
/// exclusion logic is exercised for real rather than trivially satisfied by the rule being off.
/// </remarks>
public class Rsa0009Tests
{
    private static readonly ImmutableArray<MetadataReference> References =
    [
        .. ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
           .Split(Path.PathSeparator)
           .Select(path => MetadataReference.CreateFromFile(path))
    ];

    [Theory]
    [InlineData(nameof(ConcreteMethodNullableReturn), ConcreteMethodNullableReturn)]
    [InlineData(nameof(NonNullableAbstractMethodReturn), NonNullableAbstractMethodReturn)]
    [InlineData(nameof(NullableCollectionOnAbstractMethod), NullableCollectionOnAbstractMethod)]
    [InlineData(nameof(NullableTaskOnAbstractMethod), NullableTaskOnAbstractMethod)]
    [InlineData(nameof(SealedDefaultInterfaceMemberNullableReturn), SealedDefaultInterfaceMemberNullableReturn)]
    [InlineData(nameof(PrivateProtectedAbstractMethodNullableReturn), PrivateProtectedAbstractMethodNullableReturn)]
    [InlineData(nameof(PublicAbstractMethodOnInternalNestedType), PublicAbstractMethodOnInternalNestedType)]
    [InlineData(nameof(PublicAbstractMethodOnPrivateNestedType), PublicAbstractMethodOnPrivateNestedType)]
    [InlineData(nameof(PublicAbstractMethodOnFileScopedType), PublicAbstractMethodOnFileScopedType)]
    public async Task GivenCorrect_WhenAnalyze_ThenNoDiagnosticsReported(string name, string source)
    {
        // Given, When
        var diagnostics = await AnalyzeAsync(source);

        // Then
        diagnostics.Should().BeEmpty(because: $"{name} should not report RSA0009");
    }

    [Theory]
    [InlineData(OverrideNullableReturn)]
    [InlineData(ExplicitInterfaceImplementationNullableReturn)]
    public async Task GivenOverrideOrExplicitImplementation_WhenAnalyze_ThenOnlyOriginDeclarationReported(string source)
    {
        // Given, When
        var diagnostics = await AnalyzeAsync(source);

        // Then. The origin abstract/interface member reports; the override and the explicit
        // implementation do not, since neither is itself abstract or virtual.
        diagnostics.Should().ContainSingle();
    }

    [Fact]
    public async Task GivenReabstractingOverride_WhenAnalyze_ThenOnlyOriginDeclarationReported()
    {
        // Given, When. A re-abstracting `abstract override` is both an override
        // (IsInheritedContract) and, per IsAbstract, an abstraction origin at the same time. Only
        // the member that actually originated the nullable return - Base.Read - should report;
        // Middle.Read merely restates the same contract it inherited.
        var diagnostics = await AnalyzeAsync(ReabstractingOverrideNullableReturn);

        // Then
        diagnostics.Should().ContainSingle();
    }

    [Theory]
    [InlineData(nameof(AbstractMethodNullableReturn), AbstractMethodNullableReturn)]
    [InlineData(nameof(AbstractPropertyNullableReturn), AbstractPropertyNullableReturn)]
    [InlineData(nameof(VirtualMethodNullableReturn), VirtualMethodNullableReturn)]
    [InlineData(nameof(DefaultInterfaceMemberNullableReturn), DefaultInterfaceMemberNullableReturn)]
    [InlineData(nameof(StaticAbstractInterfaceMemberNullableReturn), StaticAbstractInterfaceMemberNullableReturn)]
    [InlineData(nameof(AbstractIndexerNullableReturn), AbstractIndexerNullableReturn)]
    public async Task GivenIncorrect_WhenAnalyze_ThenDiagnosticsReported(string name, string source)
    {
        // Given, When
        var diagnostics = await AnalyzeAsync(source);

        // Then
        diagnostics.Should().ContainSingle(because: $"{name} should report RSA0009");
    }

    private static async Task<System.Collections.Generic.IReadOnlyList<Diagnostic>> AnalyzeAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));

        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
           .WithNullableContextOptions(NullableContextOptions.Enable)
           .WithSpecificDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic>.Empty.Add(RSA0009.Id, ReportDiagnostic.Info));

        var compilation = CSharpCompilation.Create("Rsa0009Tests", [tree], References, options);

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new Rsa0009());
        var withAnalyzers = compilation.WithAnalyzers(analyzers);
        var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync();

        return diagnostics.Where(diagnostic => diagnostic.Id == RSA0009.Id).ToList();
    }

    // lang=csharp
    internal const string ConcreteMethodNullableReturn =
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
    internal const string NonNullableAbstractMethodReturn =
        """
        namespace Sample
        {
            public abstract class Base
            {
                public abstract string Read();
            }
        }
        """;

    // lang=csharp
    internal const string NullableCollectionOnAbstractMethod =
        """
        using System.Collections.Generic;

        namespace Sample
        {
            public abstract class Base
            {
                public abstract IEnumerable<string>? Read();
            }
        }
        """;

    // lang=csharp
    internal const string NullableTaskOnAbstractMethod =
        """
        using System.Threading.Tasks;

        namespace Sample
        {
            public abstract class Base
            {
                public abstract Task<string>? ReadAsync();
            }
        }
        """;

    // lang=csharp
    internal const string SealedDefaultInterfaceMemberNullableReturn =
        """
        namespace Sample
        {
            public interface IReader
            {
                sealed string? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string PrivateProtectedAbstractMethodNullableReturn =
        """
        namespace Sample
        {
            public abstract class Base
            {
                private protected abstract string? Read();
            }
        }
        """;

    // lang=csharp
    internal const string PublicAbstractMethodOnInternalNestedType =
        """
        namespace Sample
        {
            public class Outer
            {
                internal abstract class Base
                {
                    public abstract string? Read();
                }
            }
        }
        """;

    // lang=csharp
    internal const string PublicAbstractMethodOnPrivateNestedType =
        """
        namespace Sample
        {
            public class Outer
            {
                private abstract class Base
                {
                    public abstract string? Read();
                }
            }
        }
        """;

    // lang=csharp
    internal const string PublicAbstractMethodOnFileScopedType =
        """
        file abstract class Base
        {
            public abstract string? Read();
        }
        """;

    // lang=csharp
    internal const string OverrideNullableReturn =
        """
        namespace Sample
        {
            public abstract class Base
            {
                public abstract string? Read();
            }

            public class Example : Base
            {
                public override string? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string ReabstractingOverrideNullableReturn =
        """
        namespace Sample
        {
            public abstract class Base
            {
                public abstract string? Read();
            }

            public abstract class Middle : Base
            {
                public abstract override string? Read();
            }

            public class Derived : Middle
            {
                public override string? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string ExplicitInterfaceImplementationNullableReturn =
        """
        namespace Sample
        {
            public interface IReader
            {
                string? Read();
            }

            public class Example : IReader
            {
                string? IReader.Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string AbstractMethodNullableReturn =
        """
        namespace Sample
        {
            public abstract class Base
            {
                public abstract string? Read();
            }
        }
        """;

    // lang=csharp
    internal const string AbstractPropertyNullableReturn =
        """
        namespace Sample
        {
            public abstract class Base
            {
                public abstract string? Value { get; }
            }
        }
        """;

    // lang=csharp
    internal const string VirtualMethodNullableReturn =
        """
        namespace Sample
        {
            public class Base
            {
                public virtual string? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string DefaultInterfaceMemberNullableReturn =
        """
        namespace Sample
        {
            public interface IReader
            {
                string? Read() => null;
            }
        }
        """;

    // lang=csharp
    internal const string StaticAbstractInterfaceMemberNullableReturn =
        """
        namespace Sample
        {
            public interface IParser<T>
                where T : class
            {
                static abstract T? Parse(string value);
            }
        }
        """;

    // lang=csharp
    internal const string AbstractIndexerNullableReturn =
        """
        namespace Sample
        {
            public abstract class Base
            {
                public abstract string? this[int index] { get; }
            }
        }
        """;
}
