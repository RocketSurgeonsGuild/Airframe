using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.Airframe.Defaults.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsad0002 : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override void Initialize(AnalysisContext analysisContext)
    {
        analysisContext.EnableConcurrentExecution();
        analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        analysisContext.RegisterSyntaxNodeAction(
            action: context =>
            {
                var classDeclaration = (ClassDeclarationSyntax)context.Node;
                var hasAttribute = classDeclaration.AttributeLists.Any(listSyntax => listSyntax.Attributes.Any(syntax => syntax.IsDefaultsAttribute()));
                var constructors = classDeclaration.Members.OfType<ConstructorDeclarationSyntax>().ToImmutableList();

                if (!hasAttribute || constructors.Count <= 1)
                {
                    return;
                }

                foreach (var constructor in constructors)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptions.Rsad0002, constructor.GetLocation()));
                }
            },
            syntaxKinds: SyntaxKind.ClassDeclaration);
    }

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [DiagnosticDescriptions.Rsad0002];
}