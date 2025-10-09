using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Rocket.Surgery.Airframe.Defaults.Factory;

[Generator]
internal class FactoryGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxProvider = context.SyntaxProvider
           .ForAttributeWithMetadataName(
                "Rocket.Surgery.Airframe.Defaults.DefaultsFactoryAttribute",
                (node, _) => node.IsKind(SyntaxKind.ClassDeclaration),
                (syntaxContext, _) => syntaxContext)
           .Combine(context.CompilationProvider);

        context.RegisterSourceOutput(syntaxProvider, GenerateFactories);
    }

    private void GenerateFactories(
        SourceProductionContext sourceProductionContext,
        (GeneratorAttributeSyntaxContext SyntaxContext, Compilation Compilation) tuple)
    {
        void GenerateDefaultFactory(
            INamedTypeSymbol namedTypeSymbol,
            Compilation compilation,
            SourceProductionContext sourceProductionContext,
            GeneratorAttributeSyntaxContext generatorAttributeSyntaxContext)
        {
        }
    }
}