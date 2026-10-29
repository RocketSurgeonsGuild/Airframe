using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// The single predicate for "is this trivia a comment or an XML doc comment" — <c>//</c>,
/// <c>/* */</c>, <c>///</c>, and <c>/** */</c>. Shared by <see cref="DocumentWalk"/>'s
/// code-content measurement (RSA2013) and <c>MemberOrderFix</c>'s blank-line exemption check, so
/// the four <see cref="SyntaxTrivia.IsKind"/> checks live in exactly one place. Bug #372 was
/// caused by two independent copies of a trivia routine diverging; this exists so a third copy
/// never gets the chance to.
/// </summary>
internal static class CommentTrivia
{
    /// <summary>
    /// Whether <paramref name="trivia"/> is a single-line comment, a multi-line comment, a
    /// single-line (<c>///</c>) documentation comment, or a multi-line (<c>/** */</c>)
    /// documentation comment.
    /// </summary>
    /// <param name="trivia">The trivia to test.</param>
    /// <returns><see langword="true"/> when the trivia is a comment or doc-comment.</returns>
    public static bool IsCommentOrDoc(SyntaxTrivia trivia) =>
        trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
        trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
        trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
        trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia);
}
