using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Airframe.Defaults;

[Generator]
internal partial class DefaultsGenerator : IIncrementalGenerator
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

            // .WithComparer()

            // using StringWriter writer = new(builder, CultureInfo.InvariantCulture);
            // using IndentedTextWriter textWriter = new IndentedTextWriter(writer, "  ");
            // textWriter.Indent++;
            // textWriter.WriteLineNoTabs(null!);
            // textWriter.Indent--;

            // https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.cookbook.md
            // https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.cookbook.md#use-an-indented-text-writer-not-syntaxnodes-for-generation
            /*
             // build/MyNuGetPackageName.props
             <ItemGroup>
                   <CompilerVisibleProperty Include="MyGenerator_EnableLogging" />
                   <CompilerVisibleItemMetadata Include="AdditionalFiles" MetadataName="MyGenerator_EnableLogging" />
               </ItemGroup>
             */

            /*
             * Asembly1 .. Microsoft.CodeAnalysis.CSharp
             * MyAnalyzer : DiagnosticAnalyzer | MySuppressors : DiagnosticSuppressor | MyGenerator : IIncrementalGenerator
             * Assembly2 .. Microsoft.CodeAnalysis.CSharp.Workspaces
             * MyCodeFixer : CodeFixProvider | MyCodeRefactoring : RefactoringProvider
             */

            incrementalContext.RegisterSourceOutput(syntaxProvider, GenerateDefaults);
        }

        void RegisterDefaultAttribute(IncrementalGeneratorInitializationContext incrementalContext) => incrementalContext.RegisterPostInitializationOutput(
            initializationContext => initializationContext.AddSource($"{nameof(DefaultsAttribute)}.g.cs", DefaultsAttribute.Source));

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
            // Get properties and types
            // Find constructors that match the types found
            // Generate Default property
            // assign constructor
            var className = SymbolEqualityComparer.Default.Equals(
                namedTypeSymbol.ContainingSymbol,
                namedTypeSymbol.ContainingNamespace)
                ? namedTypeSymbol.Name
                : namedTypeSymbol.ContainingSymbol.Name + "." + namedTypeSymbol.Name;

            var attributeData =
                generatorAttributeSyntaxContext
                   .Attributes
                   .First(data => data.AttributeClass?.OriginalDefinition.ToString().Equals(DefaultsAttribute.AttributeName) ?? false);

            // Property Information
            // read only
            // =>
            // { get; }
            // Constructor
            // Multiple
            var generatedNamedTypeSymbol = generatorAttributeSyntaxContext.TargetSymbol as INamedTypeSymbol;
            var constructor = generatedNamedTypeSymbol?.Constructors.ToList();

            // ArgumentList with the Default for the Reference Type
            // ReferenceType.Default doesn't exist, Generate it? Diagnostic?
            var allConstructorParameters = constructor?.First()?.Parameters;
            var constructorArguments = BuildConstructorWithArguments(constructor?.First(), compilation);

            var memberList = constructor?.First()
              ?.Parameters.ToArray()
               .SelectMany(
                    parameterSymbol => compilation.GetTypeByMetadataName(parameterSymbol.Type.ToDisplayString())
                      ?.GetMembers()
                       .Where(symbol => symbol is IPropertySymbol && symbol.IsStatic)
                       .ToList())
               .Select(symbol => Argument(IdentifierName($"{symbol.ContainingType.Name}.{symbol.Name}")))
               .ToList();

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