using System.Collections.Generic;
using System.Collections.Immutable;
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
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

public class Rsa2016FixTests
{
    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public async Task GivenRedundantAliasWithComments_WhenFixed_ThenPreservesCommentsBindingsAndLineEndings(string newLine)
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var source = "using N;\n// Explain the import.\nusing Item = N.Item; // Keep this comment.\nclass Consumer { Item Value = new Item(); }\nnamespace N { public class Item { } }\n"
           .Replace("\n", newLine);
        var document = CreateDocument(workspace, source);
        var diagnostic = (await DiagnosticsAsync(document)).Should().ContainSingle().Subject;
        var actions = new List<CodeAction>();
        await new Rsa2016Fix().RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None));

        // When
        var operations = await actions.Should().ContainSingle().Subject.GetOperationsAsync(CancellationToken.None);
        var changed = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution.GetDocument(document.Id)!;

        // Then
        var text = (await changed.GetTextAsync()).ToString();
        text.Should().NotContain("using Item =").And.Contain("// Explain the import.").And.Contain("// Keep this comment.");
        text.Should().Contain("class Consumer { Item Value = new Item(); }");
        text.Replace(newLine, "").Should().NotContain("\r").And.NotContain("\n");
        (await DiagnosticsAsync(changed)).Should().BeEmpty();
        (await changed.Project.GetCompilationAsync())!.GetDiagnostics().Where(item => item.Severity == DiagnosticSeverity.Error).Should().BeEmpty();
    }

    [Fact]
    public async Task GivenSeveralRedundantAliases_WhenFixAllRuns_ThenRemovesOnlyRedundantAliasesAndConverges()
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace,
            "using N;\nusing Item = N.Item;\nusing Other = N.Other;\nusing Needed = M.Needed;\n"
          + "class Consumer { Item First = new Item(); Other Second = new Other(); Needed Third = new Needed(); }\n"
          + "namespace N { public class Item { } public class Other { } } namespace M { public class Needed { } }");
        (await DiagnosticsAsync(document)).Should().HaveCount(2);
        var provider = new Rsa2016Fix();
        var context = new FixAllContext(document, provider, FixAllScope.Document, "Remove redundant type alias", provider.FixableDiagnosticIds,
            new AliasDiagnostics(), CancellationToken.None);

        // When
        var action = await provider.GetFixAllProvider().GetFixAsync(context);
        action.Should().NotBeNull();
        var operations = await action!.GetOperationsAsync(CancellationToken.None);
        var changed = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution.GetDocument(document.Id)!;

        // Then
        (await changed.GetTextAsync()).ToString().Should().NotContain("using Item =").And.NotContain("using Other =").And.Contain("using Needed = M.Needed;");
        (await DiagnosticsAsync(changed)).Should().BeEmpty();
        (await changed.Project.GetCompilationAsync())!.GetDiagnostics().Where(item => item.Severity == DiagnosticSeverity.Error).Should().BeEmpty();
    }

    [Fact]
    public async Task GivenStaleDiagnostic_WhenAliasIsNowRequired_ThenNoFixIsOffered()
    {
        // Given
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace, "using Item = N.Item; class Consumer { Item Value = new Item(); } namespace N { public class Item { } }");
        var root = (await document.GetSyntaxRootAsync())!;
        var directive = root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax>().Single();
        var diagnostic = Diagnostic.Create(Descriptions.RSA2016, directive.GetLocation(), "Item");
        var actions = new List<CodeAction>();

        // When
        await new Rsa2016Fix().RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None));

        // Then
        actions.Should().BeEmpty();
    }

    private static Document CreateDocument(AdhocWorkspace workspace, string source) => workspace.AddProject("Aliases", LanguageNames.CSharp)
       .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
       .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
       .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
       .AddDocument("Example.cs", SourceText.From(source));

    private static async Task<ImmutableArray<Diagnostic>> DiagnosticsAsync(Document document)
    {
        var compilation = (await document.Project.GetCompilationAsync())!;
        return await compilation.WithAnalyzers([new Rsa2016()]).GetAnalyzerDiagnosticsAsync();
    }

    private sealed class AliasDiagnostics : FixAllContext.DiagnosticProvider
    {
        public override async Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken) =>
            await DiagnosticsAsync(document);

        public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken) =>
            Task.FromResult(Enumerable.Empty<Diagnostic>());

        public override async Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken) =>
            await (await project.GetCompilationAsync(cancellationToken))!.WithAnalyzers([new Rsa2016()]).GetAnalyzerDiagnosticsAsync(cancellationToken);
    }
}
