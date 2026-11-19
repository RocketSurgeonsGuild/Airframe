using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>Checks local type aliases against the semantic model with the directive removed.</summary>
internal static class RedundantAliases
{
    internal static IEnumerable<UsingDirectiveSyntax> Find(SemanticModel model, CancellationToken cancellationToken)
    {
        var root = model.SyntaxTree.GetRoot(cancellationToken);
        var directives = root.DescendantNodes().OfType<UsingDirectiveSyntax>()
           .Where(directive => directive.Alias != null && directive.GlobalKeyword.IsKind(SyntaxKind.None)).ToArray();
        if (directives.Length == 0)
        {
            yield break;
        }

        // Index once per file, including cref syntax, rather than scan the tree for every alias.
        var names = root.DescendantNodes(descendIntoTrivia: true).OfType<IdentifierNameSyntax>()
           .ToLookup(name => name.Identifier.ValueText, StringComparer.Ordinal);
        foreach (var directive in directives)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var name = directive.Alias!.Name.Identifier.ValueText;
            var candidates = names[name].AsEnumerable();
            if (name.EndsWith("Attribute", StringComparison.Ordinal))
            {
                candidates = candidates.Concat(names[name.Substring(0, name.Length - "Attribute".Length)]);
            }

            if (CanRemove(model, directive, candidates, cancellationToken))
            {
                yield return directive;
            }
        }
    }

    private static bool CanRemove(
        SemanticModel model,
        UsingDirectiveSyntax directive,
        IEnumerable<IdentifierNameSyntax> candidates,
        CancellationToken cancellationToken)
    {
        if (model.GetDeclaredSymbol(directive, cancellationToken) is not IAliasSymbol { Target: INamedTypeSymbol target } alias
            || target.TypeKind == TypeKind.Error
            || target.NullableAnnotation == NullableAnnotation.Annotated
            || directive.NamespaceOrType is NullableTypeSyntax
            || alias.Name != target.Name
            || directive.ContainsDiagnostics
            || directive.ContainsDirectives)
        {
            return false;
        }

        var references = candidates.Where(name => SymbolEqualityComparer.Default.Equals(model.GetAliasInfo(name, cancellationToken), alias)).ToArray();

        // Unused aliases belong to IDE0005. Renamed, namespace and global aliases are intentional
        // or require a wider scope than this file-local cleanup.
        if (references.Length == 0)
        {
            return false;
        }

        var root = model.SyntaxTree.GetRoot(cancellationToken);
        var annotation = new SyntaxAnnotation();
        var annotated = root.ReplaceNodes(references, (_, rewritten) => rewritten.WithAdditionalAnnotations(annotation));
        var toRemove = annotated.DescendantNodes().OfType<UsingDirectiveSyntax>().Single(node => node.Span == directive.Span);
        var changedRoot = annotated.RemoveNode(toRemove, SyntaxRemoveOptions.KeepExteriorTrivia)!;
        var changedTree = model.SyntaxTree.WithRootAndOptions(changedRoot, model.SyntaxTree.Options);
        var changedCompilation = model.Compilation.ReplaceSyntaxTree(model.SyntaxTree, changedTree);
        var changedModel = changedCompilation.GetSemanticModel(changedTree);

        // Resolve the original target in the new compilation. Source symbols from two compilations
        // cannot be compared directly; reference IDs also preserve constructed generic arguments.
        var referenceId = DocumentationCommentId.CreateReferenceId(target);
        var targets = DocumentationCommentId.GetSymbolsForReferenceId(referenceId, changedCompilation)
           .Where(symbol => symbol.ContainingAssembly?.Identity.Equals(target.ContainingAssembly.Identity) == true).ToArray();
        if (targets.Length != 1)
        {
            return false;
        }

        foreach (var reference in changedTree.GetRoot(cancellationToken).GetAnnotatedNodes(annotation))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var info = changedModel.GetSymbolInfo(reference, cancellationToken);
            var symbol = info.Symbol is IMethodSymbol { MethodKind: MethodKind.Constructor } constructor
                && reference.Parent is AttributeSyntax
                    ? constructor.ContainingType
                    : info.Symbol;
            if (info.CandidateReason != CandidateReason.None
                || !SymbolEqualityComparer.Default.Equals(symbol, targets[0])
                || symbol!.ToDisplayString(QualifiedTypeFormat) != target.ToDisplayString(QualifiedTypeFormat))
            {
                return false;
            }
        }

        return true;
    }

    private static readonly SymbolDisplayFormat QualifiedTypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
       .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
}
