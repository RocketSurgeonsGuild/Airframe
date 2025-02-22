using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
            var className = SymbolEqualityComparer.Default.Equals(
                namedTypeSymbol.ContainingSymbol,
                namedTypeSymbol.ContainingNamespace)
                ? namedTypeSymbol.Name
                : namedTypeSymbol.ContainingSymbol.Name + "." + namedTypeSymbol.Name;

            var attributeData = generatorAttributeSyntaxContext.Attributes.First(data => data.AttributeClass.OriginalDefinition.ToString().Equals(DefaultsAttribute.AttributeName));
            sourceProductionContext.AddSource(
                $"{className}.Defaults.g.cs",
                CompilationUnit()
                   .WithMembers(
                        GeneratePartialClassWithProperty(
                            className,
                            namedTypeSymbol,
                            attributeData))
                   .NormalizeWhitespace()
                   .ToFullString());
        }

        SyntaxList<MemberDeclarationSyntax> GeneratePartialClassWithProperty(string className, INamedTypeSymbol namedTypeSymbol, AttributeData attribute)
        {
            return SingletonList<MemberDeclarationSyntax>(
                BuildNamespace(namedTypeSymbol)
                   .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            ClassDeclaration(className)
                               .WithModifiers(
                                    TokenList(
                                        new[]
                                        {
                                            Token(SyntaxKind.PublicKeyword),
                                            Token(SyntaxKind.PartialKeyword)
                                        }))
                               .WithMembers(
                                    SingletonList<MemberDeclarationSyntax>(
                                        PropertyDeclaration(
                                                IdentifierName(className),
                                                Identifier("Default"))
                                           .WithModifiers(
                                                TokenList(
                                                    new[]
                                                    {
                                                        Token(SyntaxKind.PublicKeyword),
                                                        Token(SyntaxKind.StaticKeyword)
                                                    }))
                                           .WithAccessorList(
                                                AccessorList(
                                                    SingletonList<AccessorDeclarationSyntax>(
                                                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))))
                                           .WithInitializer(
                                                EqualsValueClause(
                                                    ObjectCreationExpression(IdentifierName(className))
                                                       .WithArgumentList(ArgumentList())))
                                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))))));
        }

        NamespaceDeclarationSyntax BuildNamespace(ISymbol namedTypeSymbol)
        {
            var displayString = namedTypeSymbol.ContainingNamespace.ToDisplayString();
            return NamespaceDeclaration(ParseName(displayString))
               .WithNamespaceKeyword(
                    Token(
                        TriviaList(LineFeed),
                        SyntaxKind.NamespaceKeyword,
                        TriviaList(Space)))
               .WithOpenBraceToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.OpenBraceToken,
                        TriviaList(LineFeed)));
        }
    }
}