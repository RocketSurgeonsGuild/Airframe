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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Performance;
using Rocket.Surgery.Airframe.CodeFixes.Performance;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Performance;

public class Rsa3002WorkspaceTests
{
    [Theory]
    [InlineData("Func<int, int> Value = argument => argument + 1;", "static argument =>")]
    [InlineData("Func<int> Value = () => 42;", "static () =>")]
    [InlineData("Func<int> Value = delegate { return 42; };", "static delegate")]
    [InlineData("Func<int, int> Value = delegate(int argument) { return argument + 1; };", "static delegate (int argument)")]
    [InlineData("Func<Task<int>> Value = async () => { await Task.Yield(); return 42; };", "static async () =>")]
    [InlineData("Func<Task<int>> Value = async delegate { await Task.Yield(); return 42; };", "static async delegate")]
    [InlineData("Expression<Func<int>> Value = () => 42;", "static () =>")]
    [InlineData("Func<int, Func<int>> Value = argument => () => argument;", "static argument =>")]
    [InlineData("Func<int> Value = () => { int Local() => 42; return Local(); };", "static () =>")]
    [InlineData("Func<int> Make() { static int Local() => 42; return delegate { return Local(); }; }", "static delegate")]
    [InlineData("int Field; Func<string> Make() => () => nameof(this.Field);", "static () => nameof(this.Field)")]
    [InlineData("Func<string> Make() { int local = 42; return () => nameof(local); }", "static () => nameof(local)")]
    public async Task GivenEligibleFunction_WhenFixed_ThenStaticFunctionCompilesAndConverges(string member, string expected)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace, "using System; using System.Threading.Tasks; using System.Linq.Expressions; class Example { " + member + " }");
        await AssertCompilesAsync(document);
        var diagnostic = (await DiagnosticsAsync(document)).Should().ContainSingle().Subject;

        // When
        var actions = new List<CodeAction>();
        await new Rsa3002Fix().RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None));
        var changed = await ApplyAsync(document, actions.Should().ContainSingle().Subject);

        // Then
        (await changed.GetTextAsync()).ToString().Should().Contain(expected);
        await AssertCompilesAsync(changed);
        (await DiagnosticsAsync(changed)).Should().BeEmpty();
    }

    [Theory]
    [InlineData(FixAllScope.Document)]
    [InlineData(FixAllScope.Project)]
    [InlineData(FixAllScope.Solution)]
    public async Task GivenMixedAndNestedFunctions_WhenFixAllRuns_ThenOnlyTheSelectedScopeChanges(FixAllScope scope)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var first = CreateDocument(workspace,
            "using System; class First { Func<Func<int>> Value = () => delegate { return 42; }; Func<int> Captures() { int local = 1; return () => local; } }");
        var second = first.Project.AddDocument("Second.cs", "using System; class Second { Func<int> Value = delegate { return 42; }; }");
        var other = CreateDocument(workspace, "using System; class Third { Func<int> Value = () => 42; }", "Other");
        var solution = second.Project.Solution.AddProject(other.Project.Id, "Other", "Other", LanguageNames.CSharp)
           .WithProjectCompilationOptions(other.Project.Id, other.Project.CompilationOptions!)
           .WithProjectParseOptions(other.Project.Id, other.Project.ParseOptions!)
           .AddMetadataReferences(other.Project.Id, other.Project.MetadataReferences)
           .AddDocument(other.Id, "Third.cs", (await other.GetTextAsync()));
        first = solution.GetDocument(first.Id)!;

        // When
        var changed = await FixAllAsync(first, scope);

        // Then
        (await DiagnosticsAsync(changed)).Should().BeEmpty();
        var changedSecond = changed.Project.Solution.GetDocument(second.Id)!;
        var changedOther = changed.Project.Solution.GetDocument(other.Id)!;
        (await DiagnosticsAsync(changedSecond)).Length.Should().Be(scope == FixAllScope.Document ? 1 : 0);
        (await DiagnosticsAsync(changedOther)).Length.Should().Be(scope == FixAllScope.Solution ? 0 : 1);
        foreach (var project in changed.Project.Solution.Projects)
        {
            (await project.GetCompilationAsync())!.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).Should().BeEmpty();
        }
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public async Task GivenFunctionWithComments_WhenFixed_ThenCommentsAndLineEndingsArePreserved(string newLine)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var source = "using System;\nclass Example\n{\n    Func<int> Value = /* before */ delegate /* keyword */ () { return 42; }; // after\n}\n"
           .Replace("\n", newLine);
        var document = CreateDocument(workspace, source);

        // When
        var changed = await FixAllAsync(document, FixAllScope.Document);

        // Then
        var text = (await changed.GetTextAsync()).ToString();
        text.Should().Contain("/* before */ static delegate /* keyword */").And.Contain("// after");
        text.Replace(newLine, "").Should().NotContain("\r").And.NotContain("\n");
        await AssertCompilesAsync(changed);
    }

    [Theory]
    [InlineData("Func<int> value = () => instance;", LanguageVersion.CSharp9)]
    [InlineData("Func<int> value = delegate { return instance; };", LanguageVersion.CSharp9)]
    [InlineData("Func<int> value = static () => 42;", LanguageVersion.CSharp9)]
    [InlineData("Func<int> value = () => 42;", LanguageVersion.CSharp8)]
    public async Task GivenStaleDiagnostic_WhenFunctionIsIneligible_ThenNoFixIsOffered(string statements, LanguageVersion version)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace, "using System; class Example { int instance; void Method() { " + statements + " } }");
        document = document.Project.WithParseOptions(new CSharpParseOptions(version)).GetDocument(document.Id)!;
        var function = (await document.GetSyntaxRootAsync())!.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>().Single();
        var diagnostic = Diagnostic.Create(Descriptions.RSA3002, function.GetLocation());
        var actions = new List<CodeAction>();

        // When
        await new Rsa3002Fix().RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None));

        // Then
        actions.Should().BeEmpty();
    }

    internal static Document CreateDocument(AdhocWorkspace workspace, string source, string projectName = "StaticFunctions") => workspace.AddProject(projectName, LanguageNames.CSharp)
       .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
       .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
       .AddMetadataReferences(((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
           .Select(path => MetadataReference.CreateFromFile(path)))
       .AddDocument("Example.cs", SourceText.From(source));

    internal static async Task<ImmutableArray<Diagnostic>> DiagnosticsAsync(Document document)
    {
        var tree = await document.GetSyntaxTreeAsync();
        var diagnostics = await (await document.Project.GetCompilationAsync())!.WithAnalyzers([new Rsa3002()]).GetAnalyzerDiagnosticsAsync();
        return diagnostics.Where(diagnostic => diagnostic.Location.SourceTree == tree).ToImmutableArray();
    }

    internal static async Task AssertCompilesAsync(Document document) => (await document.Project.GetCompilationAsync())!
       .GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

    internal static async Task<Document> FixAllAsync(Document document, FixAllScope scope)
    {
        var provider = new Rsa3002Fix();
        var context = new FixAllContext(document, provider, scope, "Make lambda expression static", provider.FixableDiagnosticIds,
            new FunctionDiagnostics(), CancellationToken.None);
        var action = await provider.GetFixAllProvider().GetFixAsync(context);
        action.Should().NotBeNull();
        return await ApplyAsync(document, action!);
    }

    private static async Task<Document> ApplyAsync(Document document, CodeAction action) => (await action.GetOperationsAsync(CancellationToken.None))
       .OfType<ApplyChangesOperation>().Single().ChangedSolution.GetDocument(document.Id)!;

    private sealed class FunctionDiagnostics : FixAllContext.DiagnosticProvider
    {
        public override async Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken) => await DiagnosticsAsync(document);

        public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken) => Task.FromResult(Enumerable.Empty<Diagnostic>());

        public override async Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken) =>
            await (await project.GetCompilationAsync(cancellationToken))!.WithAnalyzers([new Rsa3002()]).GetAnalyzerDiagnosticsAsync(cancellationToken);
    }
}
