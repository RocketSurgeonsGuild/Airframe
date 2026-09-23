using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// The adjacent member walk shared by RSA2001 through RSA2007.
/// </summary>
/// <remarks>
/// RSA2001 through RSA2007 are seven separate analyzers, each registered on the same type
/// declaration node kinds, and each owns exactly one <see cref="RankComponent"/>. Run together,
/// as they are in a real analysis pass, that would walk <c>type.Members</c> and recompute
/// <see cref="MemberRank.Of"/> for every member seven times over. Instead, the walk runs once per
/// type declaration and the per-member violations it finds are cached, keyed by the syntax node,
/// so the six analyzers that arrive after the first one reuse the same result instead of
/// recomputing it.
/// </remarks>
internal static class MemberOrderWalk
{
    private static readonly ConditionalWeakTable<TypeDeclarationSyntax, IReadOnlyList<Violation>> Cache = new();

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

        foreach (var violation in Cache.GetValue(type, static t => Walk(t)))
        {
            if (violation.Component != component)
            {
                continue;
            }

            context.ReportDiagnostic(
                Diagnostic.Create(
                    descriptor,
                    violation.Location,
                    violation.MemberName,
                    violation.MemberDescription,
                    violation.PreviousName,
                    violation.PreviousDescription));
        }
    }

    /// <summary>
    /// Walks <paramref name="type"/>'s members once, computing <see cref="MemberRank.Of"/> once per
    /// member, and records one violation per member that sorts before its immediate predecessor.
    /// This is the single pass every RSA2001-2007 analyzer draws from.
    /// </summary>
    /// <param name="type">The type declaration to walk.</param>
    /// <returns>The violations found, in member order.</returns>
    private static IReadOnlyList<Violation> Walk(TypeDeclarationSyntax type)
    {
        List<Violation>? violations = null;

        MemberDeclarationSyntax? previous = null;
        var previousRank = default(MemberRank);

        foreach (var member in type.Members)
        {
            var rank = MemberRank.Of(member);

            // Compare against the immediately preceding member only. One misplaced member
            // therefore produces one diagnostic instead of cascading down the file.
            if (previous != null && rank.CompareTo(previousRank) < 0)
            {
                violations ??= [];
                violations.Add(
                    new Violation(
                        rank.FirstDifference(previousRank),
                        MemberRank.LocationOf(member),
                        MemberRank.NameOf(member),
                        rank.Describe(),
                        MemberRank.NameOf(previous),
                        previousRank.Describe()));
            }

            previous = member;
            previousRank = rank;
        }

        return violations ?? (IReadOnlyList<Violation>)Array.Empty<Violation>();
    }

    /// <summary>
    /// One member that sorts before the member immediately preceding it.
    /// </summary>
    private readonly struct Violation
    {
        public Violation(
            RankComponent component,
            Location location,
            string memberName,
            string memberDescription,
            string previousName,
            string previousDescription)
        {
            Component = component;
            Location = location;
            MemberName = memberName;
            MemberDescription = memberDescription;
            PreviousName = previousName;
            PreviousDescription = previousDescription;
        }

        public RankComponent Component { get; }

        public Location Location { get; }

        public string MemberName { get; }

        public string MemberDescription { get; }

        public string PreviousName { get; }

        public string PreviousDescription { get; }
    }
}
