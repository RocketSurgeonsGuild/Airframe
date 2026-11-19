using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;

namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Performance;

public class Rsa3002SafetyTests
{
    [Theory]
    [InlineData("Func<int> value = delegate { return 42; };")]
    [InlineData("Func<int, int> value = delegate(int argument) { return argument + 1; };")]
    [InlineData("Func<int> value = delegate() { return StaticValue; };")]
    [InlineData("Func<int> value = () => StaticValue;")]
    [InlineData("Func<int> value = () => { int Local() => 42; return Local(); };")]
    [InlineData("Func<int, Func<int>> value = argument => () => argument;")]
    [InlineData("Func<int> value = () => { int local = 42; Func<int> nested = () => local; return nested(); };")]
    [InlineData("int local = 42; Func<int> capturing = () => local; Func<int> value = () => 42;")]
    [InlineData("int local = 42; Func<int> capturing = () => local; Func<int> value = delegate { return 42; };")]
    [InlineData("int Local() => InstanceValue; Func<string> value = () => nameof(Local);")]
    public async Task GivenEligibleAnonymousFunction_WhenAnalyzed_ThenReportsOnlyThatFunction(string statements)
    {
        // Given
        var compilation = CreateCompilation(statements);
        compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        // When
        var diagnostics = await compilation.WithAnalyzers([new Rsa3002()]).GetAnalyzerDiagnosticsAsync();

        // Then
        diagnostics.Should().ContainSingle().Which.Id.Should().Be("RSA3002");
    }

    [Theory]
    [InlineData("int local = 42; Func<int> value = delegate { return local; };")]
    [InlineData("int local = 42; Func<int> value = () => local;")]
    [InlineData("Func<int> value = delegate { return InstanceValue; };")]
    [InlineData("Func<int> value = () => this.InstanceValue;")]
    [InlineData("Func<int> value = delegate { return base.GetHashCode(); };")]
    [InlineData("Func<int> value = () => InstanceMethod();")]
    [InlineData("int Local() => 42; Func<int> value = delegate { return Local(); };")]
    [InlineData("int Local() => 42; Func<Func<int>> value = () => Local;")]
    [InlineData("Func<int> value = static delegate { return 42; };")]
    [InlineData("Func<int> value = static () => 42;")]
    public async Task GivenStaticOrCapturingFunction_WhenAnalyzed_ThenNoDiagnostic(string statements)
    {
        // Given
        var compilation = CreateCompilation(statements);
        compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        // When
        var diagnostics = await compilation.WithAnalyzers([new Rsa3002()]).GetAnalyzerDiagnosticsAsync();

        // Then
        diagnostics.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Func<int> value = delegate { return 42; };")]
    [InlineData("Func<int> value = () => 42;")]
    public async Task GivenCSharp8_WhenAnalyzed_ThenNoStaticFunctionIsSuggested(string statements)
    {
        // Given
        var compilation = CreateCompilation(statements, LanguageVersion.CSharp8);

        // When
        var diagnostics = await compilation.WithAnalyzers([new Rsa3002()]).GetAnalyzerDiagnosticsAsync();

        // Then
        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenGeneratedSource_WhenAnalyzed_ThenNoCleanupIsReported()
    {
        // Given
        var compilation = CreateCompilation("Func<int> value = delegate { return 42; };");
        var tree = compilation.SyntaxTrees.Single();
        compilation = compilation.ReplaceSyntaxTree(tree, tree.WithFilePath("Example.g.cs"));

        // When
        var diagnostics = await compilation.WithAnalyzers([new Rsa3002()]).GetAnalyzerDiagnosticsAsync();

        // Then
        diagnostics.Should().BeEmpty();
    }

    private static CSharpCompilation CreateCompilation(string statements, LanguageVersion version = LanguageVersion.CSharp9) => CSharpCompilation.Create(
        "StaticFunctions",
        [CSharpSyntaxTree.ParseText("using System; class Example { static int StaticValue = 42; int InstanceValue = 42; int InstanceMethod() => 42; void Method() { " + statements + " } }",
            new CSharpParseOptions(version))],
        [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
}
