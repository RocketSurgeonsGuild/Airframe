using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Rocket.Surgery.Airframe.CodeFixes.Design;

/// <summary>Finds existing token boundaries at which an overlong line can be wrapped.</summary>
internal static class LineChopper
{
    /// <summary>
    /// Chooses the widest wrappable construct overlapping the line. Already multiline constructs
    /// remain eligible: an individual argument or a later segment of a chain can still be long.
    /// Only tokens on the originally diagnosed lines may move. Every boundary is attempted at
    /// most once, so even a formatter that declines a break cannot cause an infinite loop.
    /// </summary>
    internal static SyntaxToken[] Targets(
        SyntaxNode root,
        SourceText text,
        TextLine line,
        SyntaxAnnotation eligible,
        SyntaxAnnotation wrapped,
        bool operatorsAtEnd)
    {
        foreach (var node in root.DescendantNodes(line.Span).OrderByDescending(node => node.Span.Length))
        {
            var targets = Boundaries(node, operatorsAtEnd)
               .Where(token => token.RawKind != 0 && !token.IsMissing && token.HasAnnotation(eligible) && !token.HasAnnotation(wrapped))
               .Where(token => token.SpanStart >= line.Start && token.SpanStart < line.End)
               .Where(token => token.GetPreviousToken().Span.End > line.Start)
               .Where(token => !token.LeadingTrivia.Any(trivia => trivia.IsDirective)
                            && !token.GetPreviousToken().TrailingTrivia.Any(trivia => trivia.IsDirective))
               .Distinct()
               .ToArray();

            if (targets.Length > 0)
            {
                return targets;
            }
        }

        return [];
    }

    /// <summary>
    /// Format the construct as a whole: turning an initializer or switch multiline can require
    /// moving its braces as well as indenting the tokens at the newly inserted breaks.
    /// </summary>
    internal static TextSpan FormattingSpan(SyntaxToken token)
    {
        var node = token.Parent!.AncestorsAndSelf().First(candidate => Boundaries(candidate, operatorsAtEnd: false).Any());
        if (node is SwitchExpressionArmSyntax && node.Parent is SwitchExpressionSyntax expression)
        {
            node = expression;
        }

        // A multiline signature also changes the formatter's placement of its body brace.
        // Include that boundary so a second dotnet format run does not move it again.
        var next = node.GetLastToken().GetNextToken();
        var end = node is BaseParameterListSyntax && next.IsKind(SyntaxKind.OpenBraceToken) ? next.Span.End : node.Span.End;
        return TextSpan.FromBounds(Math.Min(node.SpanStart, token.GetPreviousToken().SpanStart), end);
    }

    /// <summary>
    /// Indent list items from their opening delimiter, including when earlier items already
    /// occupy a continuation line. Indenting from that line would add another level to later items.
    /// </summary>
    internal static int IndentationAnchor(SyntaxToken token) => ListFor(token)?.SpanStart ?? token.SpanStart;

    internal static IEnumerable<SyntaxToken> ExistingLineStarts(IEnumerable<SyntaxToken> targets, SourceText text, SyntaxAnnotation eligible) =>
        targets.Select(ListFor).OfType<SyntaxNode>().Distinct()
           .SelectMany(node => Boundaries(node, operatorsAtEnd: false))
           .Where(token => token.HasAnnotation(eligible)
                        && token.GetPreviousToken().Span.End <= text.Lines.GetLineFromPosition(token.SpanStart).Start);

    private static SyntaxNode? ListFor(SyntaxToken token) =>
        token.Parent!.AncestorsAndSelf()
           .FirstOrDefault(node => node is BaseParameterListSyntax or BaseArgumentListSyntax
                               && Boundaries(node, operatorsAtEnd: false).Contains(token));

    private static IEnumerable<SyntaxToken> Boundaries(SyntaxNode node, bool operatorsAtEnd) => node switch
    {
        ParameterListSyntax list => list.Parameters.Select(item => item.GetFirstToken()),
        BracketedParameterListSyntax list => list.Parameters.Select(item => item.GetFirstToken()),
        ArgumentListSyntax list => list.Arguments.Select(item => item.GetFirstToken()),
        BracketedArgumentListSyntax list => list.Arguments.Select(item => item.GetFirstToken()),
        InitializerExpressionSyntax list => list.Expressions.Select(item => item.GetFirstToken()),
        AnonymousObjectCreationExpressionSyntax list => list.Initializers.Select(item => item.GetFirstToken()),
        CollectionExpressionSyntax list => list.Elements.Select(item => item.GetFirstToken()),
        ArrowExpressionClauseSyntax arrow => [arrow.Expression.GetFirstToken()],
        MemberAccessExpressionSyntax { Expression: InvocationExpressionSyntax } member => [member.OperatorToken],
        BinaryExpressionSyntax binary => [operatorsAtEnd ? binary.Right.GetFirstToken() : binary.OperatorToken],
        ConditionalExpressionSyntax conditional => operatorsAtEnd
            ? [conditional.WhenTrue.GetFirstToken(), conditional.WhenFalse.GetFirstToken()]
            : [conditional.QuestionToken, conditional.ColonToken],
        SwitchExpressionSyntax expression => expression.Arms.Select(arm => arm.GetFirstToken()).Append(expression.CloseBraceToken),
        SwitchExpressionArmSyntax arm => [arm.Expression.GetFirstToken()],
        BaseListSyntax list => list.Types.Select(type => type.GetFirstToken()),
        TypeParameterConstraintClauseSyntax clause => new[] { clause.WhereKeyword }.Concat(clause.Constraints.Select(item => item.GetFirstToken())),
        _ => []
    };
}
