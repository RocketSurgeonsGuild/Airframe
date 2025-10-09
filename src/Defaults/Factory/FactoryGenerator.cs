using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
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
            var className = SymbolEqualityComparer.Default.Equals(
                namedTypeSymbol.ContainingSymbol,
                namedTypeSymbol.ContainingNamespace)
                ? namedTypeSymbol.Name
                : namedTypeSymbol.ContainingSymbol.Name + "." + namedTypeSymbol.Name;

            var attributeData =
                generatorAttributeSyntaxContext
                   .Attributes
                   .First(data => data.AttributeClass?.OriginalDefinition.ToString().Equals(FactoryAttribute.AttributeName) ?? false);

            var generatedNamedTypeSymbol = generatorAttributeSyntaxContext.TargetSymbol as INamedTypeSymbol;
            var constructor = generatedNamedTypeSymbol?.Constructors.ToList();

            var constructorArguments = BuildConstructorWithArguments(constructor?.First(), compilation);

            sourceProductionContext
               .AddSource(
                    $"{className}.Factory.Defaults.g.cs",
                    CompilationUnit().WithMembers(
                            GeneratePartialClassWithProperty(
                                IdentifierName(className),
                                namedTypeSymbol,
                                attributeData,
                                constructorArguments))
                       .NormalizeWhitespace()
                       .ToFullString());
        }
    }

    private SyntaxList<MemberDeclarationSyntax> GeneratePartialClassWithProperty(
        IdentifierNameSyntax className,
        INamedTypeSymbol namedTypeSymbol,
        AttributeData attribute,
        ArgumentListSyntax? argumentListSyntax)
    {
        var propertyName = attribute.NamedArguments.FirstOrDefault(pair => pair.Key == "PropertyName").Value.Value?.ToString() ?? "Default";
        return SingletonList<MemberDeclarationSyntax>(
            BuildNamespace(namedTypeSymbol)
               .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        ClassDeclaration(className.Identifier)
                           .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.PartialKeyword)))
                           .WithMembers(
                                SingletonList<MemberDeclarationSyntax>(
                                    PropertyDeclaration(
                                            className,
                                            Identifier(propertyName))
                                       .WithModifiers(
                                            TokenList(
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.StaticKeyword)))
                                       .WithAccessorList(
                                            AccessorList(
                                                SingletonList(
                                                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                                       .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))))
                                       .WithInitializer(EqualsValueClause(BuildObjectCreationExpression(className, argumentListSyntax)))
                                       .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))))));
    }
}