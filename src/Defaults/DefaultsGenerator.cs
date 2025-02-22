using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Rocket.Surgery.Airframe.Defaults;

[Generator]
public partial class DefaultsGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterDefaultsGenerator(context);
        RegisterDefaultAttribute(context);

        [SuppressMessage("ReSharper", "RedundantNameQualifier", Justification = "Source Generation")]
        void RegisterDefaultsGenerator(IncrementalGeneratorInitializationContext incrementalContext)
        {
            var syntaxProvider = incrementalContext.SyntaxProvider
               .ForAttributeWithMetadataName(
                    "Rocket.Surgery.Airframe.Defaults.DefaultsAttribute",
                    (node, _) => node.IsKind(SyntaxKind.ClassDeclaration),
                    (syntaxContext, _) => syntaxContext)
               .Combine(incrementalContext.CompilationProvider);

            incrementalContext.RegisterSourceOutput(syntaxProvider, GenerateDefaults);
        }

        void RegisterDefaultAttribute(IncrementalGeneratorInitializationContext incrementalContext) => incrementalContext.RegisterPostInitializationOutput(
            initializationContext => initializationContext.AddSource(
                "DefaultsAttribute.g.cs",
                DefaultsAttribute.Source));

        void GenerateDefaults(SourceProductionContext productionContext, (GeneratorAttributeSyntaxContext SyntaxContext, Compilation Compilation) tuple)
        {
            var (syntaxContext, compilation) = tuple;

            // does class have attribute
            var attributeTarget = GetClassForFixture(syntaxContext);

            if (attributeTarget is null)
            {
                return;
            }

            if (Report0001(attributeTarget, productionContext))
            {
                return;
            }

            GenerateDefaultProperty(attributeTarget, compilation, productionContext, syntaxContext);
        }

        void GenerateDefaultProperty(
            INamedTypeSymbol namedTypeSymbol,
            Compilation compilation,
            SourceProductionContext sourceProductionContext,
            GeneratorAttributeSyntaxContext generatorAttributeSyntaxContext)
        {
            // Get properties and types
            // Find constructors that match the types found
            // Generate Default property
            // assign constructor
        }
    }
}