using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// The adjacent member walk shared by RSA2001 through RSA2007.
/// </summary>
internal static class MemberOrderWalk
{
    /// <summary>
    /// Reports <paramref name="descriptor"/> on every member of the analyzed type that sorts before
    /// the member immediately preceding it, where the first differing rank component is
    /// <paramref name="component"/>.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="descriptor">The descriptor to report.</param>
    /// <param name="component">The rank component this diagnostic owns.</param>
    public static void Report(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, RankComponent component)
    {
        if (context.Node is not TypeDeclarationSyntax type)
        {
            return;
        }

        MemberDeclarationSyntax? previous = null;
        var previousRank = default(MemberRank);

        foreach (var member in type.Members)
        {
            var rank = MemberRank.Of(member);

            // Compare against the immediately preceding member only. One misplaced member
            // therefore produces one diagnostic instead of cascading down the file.
            if (previous != null && rank.CompareTo(previousRank) < 0 && rank.FirstDifference(previousRank) == component)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        descriptor,
                        MemberRank.LocationOf(member),
                        MemberRank.NameOf(member),
                        rank.Describe(),
                        MemberRank.NameOf(previous),
                        previousRank.Describe()));
            }

            previous = member;
            previousRank = rank;
        }
    }
}
