using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

public class MemberOrderFixTests
{
    [Theory]
    [InlineData(nameof(ConstructorAfterField), ConstructorAfterField)]
    [InlineData(nameof(Unordered), Unordered)]
    [InlineData(nameof(DocumentedMembers), DocumentedMembers)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithCodeFix<MemberOrderFix>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    [Theory]
    [InlineData(ConstructorAfterField)]
    public async Task GivenConstructorAfterField_WhenCodeFix_ThenFixResolved(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithCodeFix<MemberOrderFix>()
           .AddSources(source)
           .Build()
           .GenerateAsync();

        // Then
        result
           .CodeFixResults[typeof(MemberOrderFix)]
           .ResolvedFixes
           .Should()
           .ContainSingle()
           .Which
           .Diagnostic
           .Descriptor
           .Should()
           .Be(Descriptions.RSA2001);
    }

    // lang=csharp
    internal const string ConstructorAfterField =
        """
        namespace Sample
        {
            public class Example
            {
                private readonly int _value;

                public Example()
                {
                }
            }
        }
        """;

    // lang=csharp
    internal const string Unordered =
        """
        namespace Sample
        {
            public class Example
            {
                private void Helper()
                {
                }

                public void Method()
                {
                }

                public int Property { get; set; }

                private int _value;

                public Example()
                {
                }
            }
        }
        """;

    // lang=csharp
    internal const string DocumentedMembers =
        """
        namespace Sample
        {
            /// <summary>An example.</summary>
            public class Example
            {
                /// <summary>Helps.</summary>
                private void Helper()
                {
                }

                /// <summary>Creates the example.</summary>
                public Example()
                {
                }
            }
        }
        """;
}
