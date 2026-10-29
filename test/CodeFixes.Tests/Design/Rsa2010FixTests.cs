using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

public class Rsa2010FixTests
{
    [Theory]
    [InlineData(nameof(Regions), Regions)]
    [InlineData(nameof(NestedRegions), NestedRegions)]
    public async Task GivenSource_WhenCodeFix_ThenVerify(string name, string source)
    {
        // Given, When
        var result = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2010>()
           .WithCodeFix<Rsa2010Fix>()
           .AddNormalizedSources(source)
           .WithDiagnosticSeverity(DiagnosticSeverity.Error)
           .GenerateAsync();

        // Then
        await Verifier.Verify(result).HashParameters().UseParameters(name).DisableRequireUniquePrefix();
    }

    // lang=csharp
    internal const string Regions =
        """
        namespace Sample
        {
            public class Example
            {
                #region Helpers
                private void Helper()
                {
                }
                #endregion
            }
        }
        """;

    /// <summary>
    /// A region inside a region. Each fix must take its own directive and its own partner, not the
    /// nearest endregion.
    /// </summary>
    // lang=csharp
    internal const string NestedRegions =
        """
        namespace Sample
        {
            public class Example
            {
                #region Outer
                private void First()
                {
                }

                #region Inner
                private void Second()
                {
                }
                #endregion
                #endregion
            }
        }
        """;
}
