using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

namespace Rocket.Surgery.Airframe.CodeFixes.Design;

/// <summary>
/// Represents a code fix for the member ordering diagnostics <see cref="Descriptions.RSA2001"/>
/// through <see cref="Descriptions.RSA2007"/>.
/// </summary>
/// <remarks>
/// One fix serves all seven diagnostics because there is one sort. It is named for what it does
/// rather than for a rule number, since <c>Rsa2001Fix</c> would suggest it only resolves RSA2001.
///
/// The sort applies the whole layout. A consumer who leaves RSA2003 on but switches RSA2004 off
/// still gets members ordered by access when they invoke this fix; the ordering rules are meant to
/// be tuned by severity individually and enabled as a group.
///
/// The fix also sorts the file's <c>using</c> directives: every regular using alphabetically,
/// <c>using static</c> directives next, and alias directives last. Whether <c>System</c>
/// namespaces lead the regular group, and whether a blank line separates that group from the
/// rest, are read from the standard <c>dotnet_sort_system_directives_first</c> and
/// <c>dotnet_separate_import_directive_groups</c> editorconfig keys — the same two keys Visual
/// Studio and Rider read for the same purpose — rather than a key of this project's own. A using
/// list left out of order is the same kind of layout drift the member reorder already corrects,
/// so one fix resolves both.
/// </remarks>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberOrderFix))]
public class MemberOrderFix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
    [
        RSA2001.Id,
        RSA2002.Id,
        RSA2003.Id,
        RSA2004.Id,
        RSA2005.Id,
        RSA2006.Id,
        RSA2007.Id
    ];

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];
        var node = root.FindNode(diagnostic.Location.SourceSpan);

        // Ancestors, not AncestorsAndSelf, so a flagged nested type resolves to the type that
        // contains it rather than to itself.
        var type = node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
        if (type == null)
        {
            return;
        }

        // Reordering across a conditional block would move a member into or out of it and quietly
        // change which symbols compile it. RSA2012 reports that block in its own right; here the
        // reorder simply steps aside and leaves the edit to a human.
        if (DirectiveSpan.SpansMembers(type))
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => ReorderAsync(context.Document, root, type, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static Task<Document> ReorderAsync(Document document, SyntaxNode root, TypeDeclarationSyntax type, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Stable sort: members of equal rank keep their original relative order, so two public
        // methods are never reshuffled.
        var sorted = type
           .Members
           .Select((member, index) => (member, index, rank: MemberRank.Of(member)))
           .OrderBy(x => x.rank)
           .ThenBy(x => x.index)
           .Select(x => x.member);

        // XML docs and comments are leading trivia of the member, and attribute lists are child
        // nodes of it. Both are part of the member node, so both travel with it.
        var newLine = NewLineOf(type);
        var reordered = type
           .WithMembers(Separate(sorted, newLine))
           .WithAdditionalAnnotations(Formatter.Annotation);

        var newRoot = root.ReplaceNode(type, reordered);

        if (newRoot is CompilationUnitSyntax { Usings.Count: > 1 } compilationUnit)
        {
            var options = document.Project.AnalyzerOptions.AnalyzerConfigOptionsProvider.GetOptions(root.SyntaxTree);
            var sortSystemFirst = GetBool(options, "dotnet_sort_system_directives_first", defaultValue: true);
            var separateGroups = GetBool(options, "dotnet_separate_import_directive_groups", defaultValue: false);

            newRoot = compilationUnit
               .WithUsings(SortUsings(compilationUnit.Usings, sortSystemFirst, separateGroups, newLine))
               .WithAdditionalAnnotations(Formatter.Annotation);
        }

        return Task.FromResult(document.WithSyntaxRoot(newRoot));
    }

    private static bool GetBool(AnalyzerConfigOptions options, string key, bool defaultValue) =>
        options.TryGetValue(key, out var value) && bool.TryParse(value, out var parsed) ? parsed : defaultValue;

    /// <summary>
    /// Sorts the compilation unit's using directives: every regular using alphabetically, a
    /// <c>System</c> namespace leading that group when <paramref name="sortSystemFirst"/> says so,
    /// <c>using static</c> directives next, and alias directives last — the static-then-alias tail
    /// is not governed by either editorconfig key and stays fixed regardless. Ties keep their
    /// original relative order, the same stability the member sort above guarantees.
    /// </summary>
    private static SyntaxList<UsingDirectiveSyntax> SortUsings(SyntaxList<UsingDirectiveSyntax> usings, bool sortSystemFirst, bool separateGroups, SyntaxTrivia newLine)
    {
        var sorted = usings
           .Select((directive, index) => (directive, index))
           .OrderBy(x => BucketOf(x.directive, sortSystemFirst))
           .ThenBy(x => SortKeyOf(x.directive, sortSystemFirst), StringComparer.Ordinal)
           .ThenBy(x => x.index)
           .Select(x => x.directive);

        return Arrange(sorted, directive => BucketOf(directive, sortSystemFirst), separateGroups, newLine);
    }

    /// <summary>
    /// Ranks a using directive into one of four buckets: a leading <c>System</c> subgroup (only
    /// when <paramref name="sortSystemFirst"/> is honored), the rest of the regular usings,
    /// <c>using static</c>, and alias. <see cref="Arrange"/> reads this same bucket to decide
    /// where a blank line belongs.
    /// </summary>
    private static int BucketOf(UsingDirectiveSyntax directive, bool sortSystemFirst)
    {
        if (directive.Alias != null)
        {
            return 3;
        }

        if (directive.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
        {
            return 2;
        }

        return sortSystemFirst && IsSystem(directive) ? 0 : 1;
    }

    private static string SortKeyOf(UsingDirectiveSyntax directive, bool sortSystemFirst)
    {
        if (directive.Alias != null)
        {
            return directive.Alias.Name.Identifier.Text;
        }

        var name = directive.Name?.ToString() ?? string.Empty;
        if (!sortSystemFirst)
        {
            return name;
        }

        return (IsSystem(directive) ? "0" : "1") + name;
    }

    private static bool IsSystem(UsingDirectiveSyntax directive)
    {
        var name = directive.Name?.ToString() ?? string.Empty;
        return name == "System" || name.StartsWith("System.", StringComparison.Ordinal);
    }

    /// <summary>
    /// Rebuilds the blank line between using directives, mirroring <see cref="Separate"/> for
    /// members. Unlike the member sort, a blank line is inserted only at a bucket boundary, and
    /// only when <paramref name="separateGroups"/> asks for one.
    /// </summary>
    private static SyntaxList<UsingDirectiveSyntax> Arrange(IEnumerable<UsingDirectiveSyntax> usings, Func<UsingDirectiveSyntax, int> bucketOf, bool separateGroups, SyntaxTrivia newLine)
    {
        var arranged = new List<UsingDirectiveSyntax>();
        var first = true;
        int? previousBucket = null;

        foreach (var directive in usings)
        {
            var leading = directive.GetLeadingTrivia();
            var index = 0;

            while (index < leading.Count)
            {
                if (leading[index].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    index++;
                    continue;
                }

                if (leading[index].IsKind(SyntaxKind.WhitespaceTrivia) &&
                    index + 1 < leading.Count &&
                    leading[index + 1].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    index += 2;
                    continue;
                }

                break;
            }

            var trimmed = SyntaxFactory.TriviaList(leading.Skip(index));
            var bucket = bucketOf(directive);

            // Unlike a member, whose trailing trivia carries only its own line ending, two usings
            // are already one line apart once the blank-line trivia above is stripped, so no
            // separator is inserted by default. A blank line is added only at a bucket boundary,
            // and only when asked for one.
            var newGroup = separateGroups && previousBucket is { } previous && previous != bucket;

            var leadingTrivia = !first && newGroup ? trimmed.Insert(0, newLine) : trimmed;

            arranged.Add(directive.WithLeadingTrivia(leadingTrivia));
            first = false;
            previousBucket = bucket;
        }

        return SyntaxFactory.List(arranged);
    }

    /// <summary>
    /// Rebuilds the blank line between members. A member's blank line is part of its leading
    /// trivia, so without this the line that used to separate the first member from the brace
    /// travels with that member and the members it was moved past end up glued together.
    /// </summary>
    private static SyntaxList<MemberDeclarationSyntax> Separate(IEnumerable<MemberDeclarationSyntax> members, SyntaxTrivia newLine)
    {
        var separated = new List<MemberDeclarationSyntax>();
        var first = true;

        foreach (var member in members)
        {
            var leading = member.GetLeadingTrivia();
            var index = 0;

            while (index < leading.Count)
            {
                if (leading[index].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    index++;
                    continue;
                }

                if (leading[index].IsKind(SyntaxKind.WhitespaceTrivia) &&
                    index + 1 < leading.Count &&
                    leading[index + 1].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    index += 2;
                    continue;
                }

                break;
            }

            var trimmed = SyntaxFactory.TriviaList(leading.Skip(index));

            separated.Add(member.WithLeadingTrivia(first ? trimmed : trimmed.Insert(0, newLine)));
            first = false;
        }

        return SyntaxFactory.List(separated);
    }

    /// <summary>
    /// Gets the line ending the type already uses, so a reorder does not introduce a second
    /// convention into the file.
    /// </summary>
    private static SyntaxTrivia NewLineOf(TypeDeclarationSyntax type)
    {
        foreach (var trivia in type.DescendantTrivia())
        {
            if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                return trivia;
            }
        }

        return SyntaxFactory.CarriageReturnLineFeed;
    }

    private const string Title = "Reorder members to match the layout";
}
