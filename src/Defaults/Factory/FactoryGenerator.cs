using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Airframe.Defaults.Factory;

[Generator]
internal class FactoryGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxProvider = context.SyntaxProvider
           .ForAttributeWithMetadataName(
                "Rocket.Surgery.Airframe.Defaults.FactoryAttribute",
                (node, _) => node.IsKind(SyntaxKind.ClassDeclaration),
                (syntaxContext, _) => syntaxContext)
           .Combine(context.CompilationProvider);

        context.RegisterSourceOutput(syntaxProvider, GenerateFactories);

        RegisterAttribute(context);

        void RegisterAttribute(IncrementalGeneratorInitializationContext incrementalContext)
            => incrementalContext.RegisterPostInitializationOutput(initializationContext
                => initializationContext.AddSource($"{nameof(FactoryAttribute)}.g.cs", FactoryAttribute.Source));
    }

    private void GenerateFactories(
        SourceProductionContext productionContext,
        (GeneratorAttributeSyntaxContext SyntaxContext, Compilation Compilation) tuple)
    {
        var (syntaxContext, compilation) = tuple;

        var attributeTarget = syntaxContext.GetClass();

        if (attributeTarget is null)
        {
            return;
        }

        GenerateDefaultFactory(attributeTarget, compilation, productionContext, syntaxContext);

        void GenerateDefaultFactory(
            INamedTypeSymbol namedTypeSymbol,
            Compilation compilation,
            SourceProductionContext sourceProductionContext,
            GeneratorAttributeSyntaxContext generatorAttributeSyntaxContext)
        {
            var attributeData =
                generatorAttributeSyntaxContext
                   .Attributes
                   .First(data => data.AttributeClass?.OriginalDefinition.ToString().Equals(FactoryAttribute.AttributeName) ?? false);

            var result =
                new { Class = "ValueTypeThing", Method = "Default", Parameters = Enumerable.Empty<object>() }.Render(FactoryTemplate.Template);

            sourceProductionContext
               .AddSource(
                    $"ValueTypeThing.Factory.g.cs",
                    result);
        }
    }
}