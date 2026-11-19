using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

public class Rsa2013WrappingTests
{
    [Theory]
    [InlineData(false, 60, false)]
    [InlineData(true, 60, false)]
    [InlineData(false, 84, true)]
    public async Task GivenLongParameterList_WhenChopped_ThenEveryParameterHasItsOwnLine(bool partiallyWrapped, int margin, bool inlineBrace)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var source = """
            class Example
            {
                public void THing(int longthing, int shortThing, int bigThing, int littleThing)
                {
                }
            }
            """;
        if (partiallyWrapped)
        {
            source = source.Replace("THing(int", "THing(\n        int");
        }

        if (inlineBrace)
        {
            source = source.Replace(")\n    {", ") {");
        }

        var document = CreateDocument(workspace, source, $"max_line_length = {margin}\n");

        // When
        var diagnostics = await DiagnosticsAsync(document);
        diagnostics.Should().ContainSingle();
        var actions = new List<CodeAction>();
        await new Rsa2013Fix().RegisterCodeFixesAsync(new CodeFixContext(
            document, diagnostics.Single(), (action, _) => actions.Add(action), CancellationToken.None));
        var operations = await actions.Single().GetOperationsAsync(CancellationToken.None);
        var changed = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution.GetDocument(document.Id)!;

        // Then
        (await changed.GetTextAsync()).ToString().Should().Be("""
            class Example
            {
                public void THing(
                    int longthing,
                    int shortThing,
                    int bigThing,
                    int littleThing)
                {
                }
            }
            """);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GivenLongArgumentList_WhenChopped_ThenEveryArgumentHasTheSameIndentation(bool partiallyWrapped)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var source = """
            class Example
            {
                string Value = string.Concat("longthing", "shortThing", "bigThing", "littleThing");
            }
            """;
        if (partiallyWrapped)
        {
            source = source.Replace("Concat(\"", "Concat(\n        \"");
        }

        var document = CreateDocument(workspace, source);

        // When
        var changed = await Rsa2013Fix.WrapAsync(document, await DiagnosticsAsync(document), CancellationToken.None);

        // Then
        (await changed.GetTextAsync()).ToString().Should().Be("""
            class Example
            {
                string Value = string.Concat(
                    "longthing",
                    "shortThing",
                    "bigThing",
                    "littleThing");
            }
            """);
    }

    [Theory]
    [ClassData(typeof(WrappingCases))]
    public async Task GivenLongConstruct_WhenWrapped_ThenOneActionPreservesCodeAndReachesTheMargin(string name, string members)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace, "using System;\nusing System.Linq;\nclass Example\n{\n" + members + "\n}\n");
        var diagnostics = await DiagnosticsAsync(document);
        diagnostics.Should().NotBeEmpty(name);
        var original = (await document.GetSyntaxRootAsync())!;

        // When
        var changed = await Rsa2013Fix.WrapAsync(document, diagnostics, CancellationToken.None);

        // Then
        (await DiagnosticsAsync(changed)).Should().BeEmpty(name + ": " + await changed.GetTextAsync());
        var root = (await changed.GetSyntaxRootAsync())!;
        root.DescendantTokens().Select(token => (token.RawKind, token.Text)).Should()
           .Equal(original.DescendantTokens().Select(token => (token.RawKind, token.Text)));
        root.DescendantTrivia().Where(CommentTrivia.IsCommentOrDoc).Select(trivia => trivia.ToFullString()).Should()
           .Equal(original.DescendantTrivia().Where(CommentTrivia.IsCommentOrDoc).Select(trivia => trivia.ToFullString()));
        (await changed.Project.GetCompilationAsync())!.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
           .Should().BeEmpty(name);
    }

    [Theory]
    [InlineData("space", "2", "4", "lf", "\n", "    ")]
    [InlineData("space", "3", "4", "crlf", "\r\n", "      ")]
    [InlineData("tab", "tab", "4", "lf", "\n", "\t\t")]
    [InlineData("tab", "tab", "8", "crlf", "\r\n", "\t\t")]
    public async Task GivenEditorConfig_WhenWrapped_ThenIndentationAndLineEndingsFollowTheDocument(
        string style, string size, string tabWidth, string ending, string newLine, string expectedIndent)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var source = "class Example\n{\n    void Method(string firstParameter, string secondParameter, string thirdParameter) { }\n}\n".ReplaceLineEndings(newLine);
        var document = CreateDocument(workspace, source, $"indent_style = {style}\nindent_size = {size}\ntab_width = {tabWidth}\nend_of_line = {ending}\n");

        // When
        var changed = await Rsa2013Fix.WrapAsync(document, await DiagnosticsAsync(document), CancellationToken.None);
        var text = (await changed.GetTextAsync()).ToString();

        // Then
        text.Should().Contain(newLine + expectedIndent + "string firstParameter,");
        text.Replace(newLine, "").Should().NotContain("\r").And.NotContain("\n");
        (await DiagnosticsAsync(changed)).Should().BeEmpty();
    }

    [Theory]
    [InlineData("beginning_of_line", "\n            &&")]
    [InlineData("end_of_line", "&&\n")]
    public async Task GivenOperatorPlacement_WhenWrapped_ThenConfiguredSideIsUsed(string placement, string expected)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace,
            "class Example\n{\n    bool Check(bool first, bool second, bool third) => first && second && third && first && second && third && first && second;\n}\n",
            $"dotnet_style_operator_placement_when_wrapping = {placement}\n");

        // When
        var changed = await Rsa2013Fix.WrapAsync(document, await DiagnosticsAsync(document), CancellationToken.None);

        // Then
        (await changed.GetTextAsync()).ToString().Should().Contain(expected);
        (await DiagnosticsAsync(changed)).Should().BeEmpty();
    }

    [Fact]
    public async Task GivenSuppressedAndUnbreakableLines_WhenWrappingOtherLines_ThenTheirTextIsPreserved()
    {
        // Given
        using var workspace = new AdhocWorkspace();
        const string untouched = "    const string Text = \"a long literal that must stay exactly as written even when it exceeds the configured margin\";";
        const string suppressed = "    void Suppressed(string firstParameter, string secondParameter, string thirdParameter) { }";
        var document = CreateDocument(workspace, "class Example\n{\n" + untouched + "\n#pragma warning disable RSA2013\n" + suppressed
            + "\n#pragma warning restore RSA2013\n    void Wrapped(string firstParameter, string secondParameter, string thirdParameter) { }\n}\n");

        // When
        var changed = await Rsa2013Fix.WrapAsync(document, await DiagnosticsAsync(document), CancellationToken.None);
        var again = await Rsa2013Fix.WrapAsync(changed, await DiagnosticsAsync(changed), CancellationToken.None);

        // Then
        var text = (await changed.GetTextAsync()).ToString();
        text.Should().Contain(untouched).And.Contain(suppressed).And.Contain("void Wrapped(\n");
        (await DiagnosticsAsync(changed)).Should().ContainSingle();
        (await again.GetTextAsync()).ToString().Should().Be(text);
    }

    [Theory]
    [InlineData("max_line_length = off")]
    [InlineData("max_line_length = unset")]
    public async Task GivenDisabledMargin_WhenAnalyzed_ThenNoWrappingIsRequested(string setting)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace, "class Example { void Method(string firstParameter, string secondParameter, string thirdParameter) { } }", setting);

        // When
        var diagnostics = await DiagnosticsAsync(document);

        // Then
        diagnostics.Should().BeEmpty();
    }

    internal static Document CreateDocument(AdhocWorkspace workspace, string source, string settings = "")
    {
        var directory = Path.Combine(Path.GetTempPath(), "rsa2013-workspace");
        var project = workspace.AddProject("Wrapping", LanguageNames.CSharp)
           .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
           .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
           .AddMetadataReferences(((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
               .Select(path => MetadataReference.CreateFromFile(path)))
           .AddAnalyzerConfigDocument(".editorconfig", SourceText.From(
                "root = true\n[*.cs]\nmax_line_length = 60\nindent_style = space\nindent_size = 4\n" + settings),
                filePath: Path.Combine(directory, ".editorconfig")).Project;
        return project.AddDocument("Example.cs", SourceText.From(source), filePath: Path.Combine(directory, "Example.cs"));
    }

    internal static async Task<ImmutableArray<Diagnostic>> DiagnosticsAsync(Document document)
    {
        var compilation = (await document.Project.GetCompilationAsync())!;
        return await compilation.WithAnalyzers([new Rsa2013()], document.Project.AnalyzerOptions).GetAnalyzerDiagnosticsAsync();
    }

    public sealed class WrappingCases : TheoryData<string, string>
    {
        public WrappingCases()
        {
            Add("fluent chain", "    int[] Values() => Enumerable.Range(0, 10).Where(value => value > 0).Select(value => value * 2).ToArray();");
            Add("boolean expression", "    bool Check(bool a, bool b, bool c) => a && b && c && a && b && c && a && b && c;");
            Add("conditional", "    string Pick(bool condition) => condition ? nameof(ArgumentException) : nameof(InvalidOperationException);");
            Add("switch arms", "    string Pick(int value) => value switch { 1 => nameof(ArgumentException), 2 => nameof(InvalidOperationException), _ => string.Empty };");
            Add("base list", "    class Child : System.Collections.Generic.List<string>, IDisposable, ICloneable\n    {\n        public void Dispose() { }\n        public object Clone() => this;\n    }");
            Add("constraints", "    T Make<T>() where T : class, IDisposable, ICloneable, new() => new T();");
            Add("nested arguments", "    string Value() => string.Concat(string.Concat(\"alpha\", \"bravo\", \"charlie\"), string.Concat(\"delta\", \"echo\", \"foxtrot\"));");
            Add("partially wrapped arguments", "    string Value() => string.Concat(\n        string.Concat(\"alpha\", \"bravo\", \"charlie\"), string.Concat(\"delta\", \"echo\", \"foxtrot\"));");
            Add("separator and element comments", "    string[] Values() => [/* opening */ \"alpha\" /* before comma */, /* after comma */ \"bravo\", /* element */ \"charlie\", \"delta\", /* trailing */]; // tail");
            Add("line comments", "    string Value() => string.Concat(\"alpha\", // first\n        /* second */ string.Concat(\"bravo\", /* nested */ \"charlie\", \"delta\", \"echo\"));");
            Add("initializer", "    object Value() => new { First = \"alpha\", Second = \"bravo\", Third = \"charlie\" };");
            Add("overlapping diagnostics", "    string Value() => string.Concat(\n        string.Concat(\"alpha\", \"bravo\", \"charlie\"), string.Concat(\"delta\", \"echo\", \"foxtrot\"),\n        string.Concat(\"golf\", \"hotel\", \"india\"), string.Concat(\"juliet\", \"kilo\", \"lima\"));");
            Add("directives", "    string Value() => string.Concat(\n#if true\n        \"alpha\", /* keep */ \"bravo\", \"charlie\", \"delta\", \"echo\",\n#endif\n        \"foxtrot\");");
            Add("parameters and body", "    string Value(string first, string second, string third) => string.Concat(first, second, third, first, second, third);");
            Add("method parameters", "    public void THing(int longthing, int shortThing, int bigThing, int littleThing) { }");
            Add("partially wrapped parameters", "    public void THing(\n        int longthing, int shortThing, int bigThing, int littleThing) { }");
        }
    }
}
