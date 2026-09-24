using FluentAssertions;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

/// <summary>
/// Regression tests for line-ending normalization across platforms.
/// Raw string literals in C# embed Environment.NewLine, causing `\r\n` on Windows
/// and `\n` on Unix. AddNormalizedSources scrubs sources to `\n` before compilation,
/// preventing Verify snapshot mismatches between platforms.
/// </summary>
public class LineEndingNormalizationTests
{
    [Fact]
    public async Task GivenSourceWithCrlfLineEndings_WhenNormalizedAndCompiled_ThenNoCarriageReturnsSurvive()
    {
        // Given
        var crlfSource = MemberOrderFixTests.ConstructorAfterField.ReplaceLineEndings("\r\n");

        // When
        var result = await GeneratorTestContextBuilder
           .Create()
           .AddNormalizedSources(crlfSource)
           .GenerateAsync();

        // Then
        result
           .InputSyntaxTrees
           .Should()
           .OnlyContain(tree => !tree.GetText().ToString().Contains('\r'));
    }

    [Fact]
    public async Task GivenCrlfAndLfSources_WhenCodeFixResolved_ThenFixedOutputsAreIdentical()
    {
        // Given
        var lfSource = MemberOrderFixTests.ConstructorAfterField;
        var crlfSource = lfSource.ReplaceLineEndings("\r\n");

        // When
        var lfResult = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithCodeFix<MemberOrderFix>()
           .AddNormalizedSources(lfSource)
           .GenerateAsync();

        var crlfResult = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithCodeFix<MemberOrderFix>()
           .AddNormalizedSources(crlfSource)
           .GenerateAsync();

        var lfFixedSource = await ResolveFixedSourceAsync(lfResult);
        var crlfFixedSource = await ResolveFixedSourceAsync(crlfResult);

        // Then. If normalization were removed, the CRLF-sourced fix would retain `\r`
        // characters the LF-sourced fix never had, and this equality would fail — the
        // same mismatch that breaks the committed LF `.verified.*` snapshots on
        // windows-latest CI.
        crlfFixedSource.Should().Be(lfFixedSource);
        crlfFixedSource.Should().NotContain("\r");
    }

    private static async Task<string> ResolveFixedSourceAsync(GeneratorTestResults result)
    {
        var resolvedFix = result.CodeFixResults[typeof(MemberOrderFix)].ResolvedFixes.Single();
        var codeAction = resolvedFix.CodeActions.Single();
        var textChange = codeAction.TextChanges.Single().Value;
        var originalText = await resolvedFix.Document.GetTextAsync();

        return originalText.WithChanges(textChange).ToString();
    }
}
