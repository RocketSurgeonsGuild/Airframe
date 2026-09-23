using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

public class Rsa2011FixTests
{
    [Theory]
    [InlineData(nameof(ImplicitType), ImplicitType)]
    [InlineData(nameof(ImplicitMember), ImplicitMember)]
    [InlineData(nameof(ImplicitWithOtherModifiers), ImplicitWithOtherModifiers)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2011>()
           .WithCodeFix<Rsa2011Fix>()
           .AddSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    [Theory]
    [InlineData(ImplicitTypeAndMember)]
    public async Task GivenTypeAndMember_WhenCodeFix_ThenEachDeclaresItsOwnDefault(string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2011>()
           .WithCodeFix<Rsa2011Fix>()
           .AddSources(source)
           .Build()
           .GenerateAsync();

        // Then. A type in a namespace defaults to internal and a member of a type to private, so
        // the two fixes must not collapse into one.
        //
        // Only the titles are asserted. When one document carries several diagnostics of the same
        // rule the harness renders every entry's TextChanges from the first applied fix, which is
        // the same quirk that leaves Rsa3002FixTests.GivenSource_WhenCodeFix_ThenVerify skipped.
        // The text each fix actually inserts is covered by the single diagnostic cases above.
        result
           .CodeFixResults[typeof(Rsa2011Fix)]
           .ResolvedFixes
           .SelectMany(fix => fix.CodeActions)
           .Select(action => action.CodeAction.Title)
           .Should()
           .BeEquivalentTo("Declare 'internal' explicitly", "Declare 'private' explicitly");
    }

    // lang=csharp
    internal const string ImplicitType =
        """
        namespace Sample
        {
            class Example
            {
                private void Method()
                {
                }
            }
        }
        """;

    // lang=csharp
    internal const string ImplicitMember =
        """
        namespace Sample
        {
            public class Example
            {
                void Method()
                {
                }
            }
        }
        """;

    // lang=csharp
    internal const string ImplicitWithOtherModifiers =
        """
        namespace Sample
        {
            public class Example
            {
                /// <summary>Does the thing.</summary>
                static void Method()
                {
                }
            }
        }
        """;

    // lang=csharp
    internal const string ImplicitTypeAndMember =
        """
        namespace Sample
        {
            class Example
            {
                void Method()
                {
                }
            }
        }
        """;
}
