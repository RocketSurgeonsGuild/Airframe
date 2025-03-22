using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Airframe.Defaults.Generator;

[Generator]
internal partial class DefaultsGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterDefaultsGenerator(context);

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

        void GenerateDefaults(SourceProductionContext productionContext, (GeneratorAttributeSyntaxContext SyntaxContext, Compilation Compilation) tuple)
        {
            var (syntaxContext, compilation) = tuple;

            // does class have attribute
            var attributeTarget = GetClassForFixture(syntaxContext);

            if (attributeTarget is null)
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
            var className = SymbolEqualityComparer.Default.Equals(
                namedTypeSymbol.ContainingSymbol,
                namedTypeSymbol.ContainingNamespace)
                ? namedTypeSymbol.Name
                : namedTypeSymbol.ContainingSymbol.Name + "." + namedTypeSymbol.Name;

            var attributeData =
                generatorAttributeSyntaxContext
                   .Attributes
                   .First(data => data.AttributeClass?.OriginalDefinition.ToString().Equals(DefaultsAttribute.AttributeName) ?? false);

            var generatedNamedTypeSymbol = generatorAttributeSyntaxContext.TargetSymbol as INamedTypeSymbol;
            var constructor = generatedNamedTypeSymbol?.Constructors.ToList();

            var constructorArguments = BuildConstructorWithArguments(constructor?.First(), compilation);

            sourceProductionContext
               .AddSource(
                    $"{className}.Defaults.g.cs",
                    CompilationUnit().WithMembers(
                            GeneratePartialClassWithProperty(
                                IdentifierName(className),
                                namedTypeSymbol,
                                attributeData,
                                constructorArguments))
                       .NormalizeWhitespace()
                       .ToFullString());
        }

        string GenerateDefaultForClass(
            IdentifierNameSyntax className,
            INamedTypeSymbol namedTypeSymbol,
            AttributeData attribute,
            ArgumentListSyntax? argumentListSyntax)
        {
            return string.Empty;
        }

        SyntaxList<MemberDeclarationSyntax> GeneratePartialClassWithProperty(
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

        NamespaceDeclarationSyntax BuildNamespace(ISymbol namedTypeSymbol)
            => NamespaceDeclaration(ParseName(namedTypeSymbol.ContainingNamespace.ToDisplayString()))
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

        ObjectCreationExpressionSyntax BuildObjectCreationExpression(
            IdentifierNameSyntax className,
            ArgumentListSyntax? argumentListSyntax) => ObjectCreationExpression(className).WithArgumentList(argumentListSyntax ?? ArgumentList());

        ArgumentListSyntax BuildConstructorWithArguments(IMethodSymbol? constructor, Compilation compilation)
        {
            if (constructor?.Parameters.Length > 0)
            {
                return ArgumentList(SeparatedList(Array.Empty<ArgumentSyntax>()));
            }

            var memberList = constructor?.Parameters.ToArray()
               .SelectMany(
                    parameterSymbol => compilation.GetTypeByMetadataName(parameterSymbol.Type.ToDisplayString())
                      ?.GetMembers()
                       .Where(symbol => symbol is IPropertySymbol && symbol.IsStatic)
                       .ToList())
               .Select(symbol => Argument(IdentifierName($"{symbol.ContainingType.Name}.{symbol.Name}")))
               .ToList();
            return ArgumentList(SeparatedList(memberList));
        }
    }
}