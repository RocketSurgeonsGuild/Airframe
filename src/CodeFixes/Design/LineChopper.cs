using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Rocket.Surgery.Airframe.CodeFixes.Design;

/// <summary>
/// Breaks an over long line at the outermost construct that can carry a line break, the way
/// Rider's chop line does.
/// </summary>
/// <remarks>
/// <para>
/// Roslyn's formatter does not wrap. It normalises whitespace it is given but has no notion of a
/// right margin, so <c>dotnet format</c> will never shorten a line on its own. Everything below is
/// the wrapping Roslyn does not do, written for the four constructs that actually carry the length
/// in this codebase: parameter lists, argument lists, collection and object initializers, and the
/// expression body of a member.
/// </para>
/// <para>
/// The shape follows what the codebase already writes by hand. A list breaks after its opening
/// token, each element sits one level deeper than the line it came from, and the closing token
/// stays on the last element's line. An expression body breaks after the arrow.
/// </para>
/// <para>
/// One chop per invocation. A line long enough to need two, such as a long signature whose body is
/// also long, shortens on the first pass and is reported again on the next, so repeated runs
/// converge rather than one pass trying to be clever.
/// </para>
/// </remarks>
internal static class LineChopper
{
    /// <summary>
    /// Produces a root with the construct covering <paramref name="line"/> chopped, or null when
    /// nothing on the line can carry a break.
    /// </summary>
    /// <param name="root">The syntax root.</param>
    /// <param name="text">The document text.</param>
    /// <param name="line">The line to shorten.</param>
    /// <returns>The rewritten root, or null.</returns>
    public static SyntaxNode? Chop(SyntaxNode root, SourceText text, TextLine line)
    {
        var indent = IndentOf(text, line);
        var target = Target(root, text, line);

        return target switch
        {
            ParameterListSyntax parameters => root.ReplaceNode(parameters, Chop(parameters, indent)),
            ArgumentListSyntax arguments => root.ReplaceNode(arguments, Chop(arguments, indent)),
            InitializerExpressionSyntax initializer => root.ReplaceNode(initializer, Chop(initializer, indent)),
            CollectionExpressionSyntax collection => root.ReplaceNode(collection, Chop(collection, indent)),
            ArrowExpressionClauseSyntax arrow => root.ReplaceNode(arrow, Chop(arrow, indent)),
            _ => null
        };
    }

    /// <summary>
    /// Picks what to break. Candidates must start on the line and still be on one line, so a
    /// construct already chopped is not chopped again. The widest one wins, because breaking the
    /// outermost construct removes the most from the line.
    /// </summary>
    /// <remarks>
    /// Only the subtree whose span overlaps <paramref name="line"/> is walked. A candidate must
    /// start on the line, so its span necessarily overlaps the line's span; any node whose span
    /// does not overlap the line therefore contains no candidate, and neither does any node whose
    /// span does not overlap it, since a child's span is always contained within its ancestors'.
    /// Scoping the walk this way is a pure performance change: it visits the same candidates the
    /// full-tree walk would have found, just without touching irrelevant subtrees.
    /// </remarks>
    /// <param name="root">The syntax root.</param>
    /// <param name="text">The document text.</param>
    /// <param name="line">The line to shorten.</param>
    /// <returns>The node to chop, or null.</returns>
    private static SyntaxNode? Target(SyntaxNode root, SourceText text, TextLine line)
    {
        SyntaxNode? widest = null;

        foreach (var node in root.DescendantNodes(line.Span, descendIntoTrivia: false))
        {
            if (!IsChoppable(node) || !StartsOn(node, line) || SpansLines(node, text))
            {
                continue;
            }

            if (widest == null || node.Span.Length > widest.Span.Length)
            {
                widest = node;
            }
        }

        return widest;
    }

    private static bool IsChoppable(SyntaxNode node) => node switch
    {
        ParameterListSyntax parameters => parameters.Parameters.Count > 0,
        ArgumentListSyntax arguments => arguments.Arguments.Count > 0,
        InitializerExpressionSyntax initializer => initializer.Expressions.Count > 0,
        CollectionExpressionSyntax collection => collection.Elements.Count > 0,
        ArrowExpressionClauseSyntax => true,
        _ => false
    };

    private static bool StartsOn(SyntaxNode node, TextLine line) =>
        node.Span.Start >= line.Start && node.Span.Start < line.End;

    /// <summary>
    /// Determines whether a node already occupies more than one line, in which case it has been
    /// chopped already and something else on the line is carrying the length.
    /// </summary>
    /// <remarks>
    /// The node's span is used rather than its text, because a node's text carries its trailing
    /// trivia: the newline after a parameter list's closing parenthesis belongs to the list, and
    /// reading that as a second line would make every parameter list look already chopped.
    /// </remarks>
    /// <param name="node">The node.</param>
    /// <param name="text">The document text.</param>
    /// <returns>A value indicating whether the node spans lines.</returns>
    private static bool SpansLines(SyntaxNode node, SourceText text) =>
        text.Lines.GetLineFromPosition(node.Span.Start).LineNumber
     != text.Lines.GetLineFromPosition(node.Span.End).LineNumber;

    private static ParameterListSyntax Chop(ParameterListSyntax list, string indent) =>
        list.WithOpenParenToken(Break(list.OpenParenToken))
           .WithParameters(Separate(list.Parameters, indent));

    private static ArgumentListSyntax Chop(ArgumentListSyntax list, string indent) =>
        list.WithOpenParenToken(Break(list.OpenParenToken))
           .WithArguments(Separate(list.Arguments, indent));

    private static InitializerExpressionSyntax Chop(InitializerExpressionSyntax initializer, string indent) =>
        initializer.WithOpenBraceToken(Break(initializer.OpenBraceToken))
           .WithExpressions(Separate(initializer.Expressions, indent));

    private static CollectionExpressionSyntax Chop(CollectionExpressionSyntax collection, string indent) =>
        collection.WithOpenBracketToken(Break(collection.OpenBracketToken))
           .WithElements(Separate(collection.Elements, indent));

    private static ArrowExpressionClauseSyntax Chop(ArrowExpressionClauseSyntax arrow, string indent) =>
        arrow.WithArrowToken(Break(arrow.ArrowToken))
           .WithExpression(arrow.Expression.WithLeadingTrivia(SyntaxFactory.Whitespace(indent + Step)));

    /// <summary>
    /// Puts every element of a list on its own line, one level deeper than the line it came from.
    /// The closing token is left where it is, so it lands on the last element's line, which is how
    /// the surrounding code is written.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="indent">The indentation of the line being chopped.</param>
    /// <returns>The separated list.</returns>
    private static SeparatedSyntaxList<T> Separate<T>(SeparatedSyntaxList<T> list, string indent)
        where T : SyntaxNode
    {
        var elements = new List<T>(list.Count);

        foreach (var element in list)
        {
            elements.Add(element.WithLeadingTrivia(SyntaxFactory.Whitespace(indent + Step)));
        }

        var separated = SyntaxFactory.SeparatedList(elements);

        // Each comma ends its line, so the element after it starts the next one.
        for (var index = 0; index < separated.SeparatorCount; index++)
        {
            separated = separated.ReplaceSeparator(
                separated.GetSeparator(index),
                separated.GetSeparator(index).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed));
        }

        return separated;
    }

    private static SyntaxToken Break(SyntaxToken token) =>
        token.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

    private static string IndentOf(SourceText text, TextLine line)
    {
        var content = text.ToString(line.Span);
        var width = 0;

        while (width < content.Length && (content[width] == ' ' || content[width] == '\t'))
        {
            width++;
        }

        return content.Substring(0, width);
    }

    private const string Step = "    ";
}
