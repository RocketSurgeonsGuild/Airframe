using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// The compilation unit walk shared by RSA2008, RSA2009, RSA2010, RSA2011, RSA2013, RSA2014 and
/// RSA2015.
/// </summary>
/// <remarks>
/// Those seven analyzers are all registered on <see cref="SyntaxKind.CompilationUnit"/>, and each
/// used to run its own independent full-document traversal: RSA2008 and RSA2009 both enumerated
/// the top level types, RSA2010 walked every trivia looking for regions, RSA2011 walked every
/// member declaration looking for missing accessibility, RSA2013 scanned every line for length,
/// and RSA2014/RSA2015 each walk every type and member looking for missing documentation. Run
/// together, as they are in a real analysis pass, that is seven passes over the same file.
/// Instead, whichever of the seven analyzers reaches a compilation unit first computes all seven
/// results in one pass and caches them, keyed by the syntax node, so the analyzers that arrive
/// after it reuse the same result instead of re-walking the document.
/// </remarks>
internal static class DocumentWalk
{
    private static readonly ConditionalWeakTable<CompilationUnitSyntax, Result> Cache = new();

    /// <summary>
    /// Gets the shared walk result for the compilation unit under analysis, computing it once and
    /// reusing it for every subsequent caller in the same analysis pass.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The walk result, empty when the context is not analyzing a compilation unit.</returns>
    public static Result Of(SyntaxNodeAnalysisContext context) =>
        context.Node is CompilationUnitSyntax compilationUnit
            ? Cache.GetValue(compilationUnit, _ => Compute(compilationUnit, context))
            : Empty;

    private static Result Compute(CompilationUnitSyntax compilationUnit, SyntaxNodeAnalysisContext context)
    {
        var topLevelTypes = TopLevelTypes.Of(compilationUnit).ToList();

        var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(compilationUnit.SyntaxTree);
        var hasLimit = TryGetLimit(options, out var configuredLimit);

        // One walk over every trivia in the tree collects both the #region directives (RSA2010)
        // and, when a line-length limit is configured, the comment/doc-comment trivia RSA2013
        // measures lines against, in source order — rather than two independent DescendantTrivia
        // enumerations over the same document, which is exactly the redundant full pass this class
        // exists to avoid. DescendantTrivia yields trivia depth-first in document order regardless
        // of descendIntoTrivia, which RSA2013's backward walk below depends on. Deliberately not
        // gating comment collection on disabled (#if/#endif-excluded) text: a comment written
        // inside inactive code is DisabledTextTrivia, not comment trivia, so it is measured and
        // reportable like any other disabled-region text — the same treatment #region and #pragma
        // directives get.
        List<SyntaxTrivia> regions = [];
        List<SyntaxTrivia> commentTrivia = [];

        foreach (var trivia in compilationUnit.DescendantTrivia(descendIntoTrivia: true))
        {
            if (trivia.IsKind(SyntaxKind.RegionDirectiveTrivia))
            {
                regions.Add(trivia);
            }
            else if (hasLimit && CommentTrivia.IsCommentOrDoc(trivia))
            {
                commentTrivia.Add(trivia);
            }
        }

        var inaccessibleMembers = compilationUnit
           .DescendantNodes()
           .OfType<MemberDeclarationSyntax>()
           .Where(Rsa2011.CanDeclareAccessibility)
           .Where(member => !HasAccessibility(member))
           .ToList();

        List<OverlongLine> overlongLines = [];
        int? limit = null;

        if (hasLimit)
        {
            limit = configuredLimit;

            var text = compilationUnit.SyntaxTree.GetText(context.CancellationToken);

            // The index of the last trivia (by source position) that has started before the
            // current line ends. Only ever advances, across the whole document, since trivia are
            // in source order and line.End only ever increases.
            var lastStarted = -1;

            foreach (var line in text.Lines)
            {
                while (lastStarted + 1 < commentTrivia.Count && commentTrivia[lastStarted + 1].SpanStart < line.End)
                {
                    lastStarted++;
                }

                var codeEnd = line.End;
                var cursor = lastStarted;

                // Walk backward through however many comment/doc trivia sit contiguously at the
                // end of this line — not just the last one. A run can be more than one trivia
                // (`/* x */ // y`), and trailing whitespace after the run's last trivia must not
                // stop the walk before it starts: the whitespace trim below is interleaved with
                // the trivia check for exactly that reason.
                while (true)
                {
                    // Whitespace trailing the code, or sitting between two trivia in the run, is
                    // not code content either.
                    while (codeEnd > line.Start && char.IsWhiteSpace(text[codeEnd - 1]))
                    {
                        codeEnd--;
                    }

                    if (codeEnd <= line.Start || cursor < 0)
                    {
                        break;
                    }

                    var trivia = commentTrivia[cursor];

                    // The run stops the moment a trivia doesn't reach all the way to what is left
                    // of the line: a gap there is real code (or some other trivia), not comment.
                    if (trivia.SpanStart >= codeEnd || trivia.Span.End < codeEnd)
                    {
                        break;
                    }

                    // Clamped to the line start when the trivia opened on an earlier line — an
                    // interior `/* */` line, or a `///` continuation line — which is exactly
                    // "this whole line is comment".
                    codeEnd = trivia.SpanStart < line.Start ? line.Start : trivia.SpanStart;
                    cursor--;
                }

                if (codeEnd <= line.Start)
                {
                    // Nothing but a trailing comment run (and the whitespace around it) on this line.
                    continue;
                }

                var length = codeEnd - line.Start;
                if (length <= configuredLimit)
                {
                    continue;
                }

                // Squiggle only the part past the margin, so the reader sees what has to go. The
                // squiggle ends at codeEnd, not the raw line end, so a trailing comment run is
                // never underlined alongside the code that pushed the line over the margin.
                overlongLines.Add(new OverlongLine(TextSpan.FromBounds(line.Start + configuredLimit, codeEnd), length));
            }
        }

        // Gated on severity, unlike the walks above: RSA2015's half in particular drives a
        // semantic query per type (FindImplementationForInterfaceMember over every interface
        // member), so a consumer who has turned both documentation rules off should not pay for
        // either walk just because some other RSA2XXX rule reached this compilation unit first.
        var undocumentedAbstractions = IsDisabled(options, "RSA2014")
            ? (IReadOnlyList<MemberDeclarationSyntax>)[]
            : compilationUnit
               .DescendantNodes()
               .OfType<MemberDeclarationSyntax>()
               .Where(Abstractions.IsAbstraction)
               .Where(member => !Abstractions.HasSummary(member))
               .ToList();

        var undocumentedImplementers = IsDisabled(options, "RSA2015")
            ? (IReadOnlyList<MemberDeclarationSyntax>)[]
            : Abstractions
               .FindImplementers(compilationUnit, context.SemanticModel)
               .Where(member => !Abstractions.HasInheritdoc(member))
               .ToList();

        return new Result(topLevelTypes, regions, inaccessibleMembers, overlongLines, limit, undocumentedAbstractions, undocumentedImplementers);
    }

    /// <summary>
    /// Determines whether <paramref name="ruleId"/> has been turned off via
    /// <c>dotnet_diagnostic.&lt;ruleId&gt;.severity</c>. A best-effort check against the one
    /// mechanism a consumer is expected to use to disable an individual rule; it does not account
    /// for a bulk category severity, a ruleset file, or a <c>#pragma</c>, so it only ever skips
    /// work it is certain nothing needs.
    /// </summary>
    private static bool IsDisabled(AnalyzerConfigOptions options, string ruleId) =>
        options.TryGetValue($"dotnet_diagnostic.{ruleId}.severity", out var severity) &&
        string.Equals(severity, "none", StringComparison.OrdinalIgnoreCase);

    private static bool HasAccessibility(MemberDeclarationSyntax member) =>
        member.Modifiers.Any(SyntaxKind.PublicKeyword)
     || member.Modifiers.Any(SyntaxKind.InternalKeyword)
     || member.Modifiers.Any(SyntaxKind.ProtectedKeyword)
     || member.Modifiers.Any(SyntaxKind.PrivateKeyword)
     || member.Modifiers.Any(SyntaxKind.FileKeyword);

    private static bool TryGetLimit(AnalyzerConfigOptions options, out int limit)
    {
        limit = 0;

        return options.TryGetValue("max_line_length", out var value)
         && !string.Equals(value, "off", StringComparison.OrdinalIgnoreCase)
         && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out limit)
         && limit > 0;
    }

    private static readonly Result Empty = new([], [], [], [], null, [], []);

    /// <summary>
    /// A line whose length exceeds the configured <c>max_line_length</c>.
    /// </summary>
    internal readonly struct OverlongLine
    {
        public OverlongLine(TextSpan span, int length)
        {
            Span = span;
            Length = length;
        }

        /// <summary>Gets the span past the margin, the part that has to go.</summary>
        public TextSpan Span { get; }

        /// <summary>Gets the full length of the line.</summary>
        public int Length { get; }
    }

    /// <summary>
    /// The result of one shared walk over a compilation unit.
    /// </summary>
    internal sealed class Result
    {
        public Result(
            IReadOnlyList<MemberDeclarationSyntax> topLevelTypes,
            IReadOnlyList<SyntaxTrivia> regions,
            IReadOnlyList<MemberDeclarationSyntax> inaccessibleMembers,
            IReadOnlyList<OverlongLine> overlongLines,
            int? limit,
            IReadOnlyList<MemberDeclarationSyntax> undocumentedAbstractions,
            IReadOnlyList<MemberDeclarationSyntax> undocumentedImplementers)
        {
            TopLevelTypes = topLevelTypes;
            Regions = regions;
            InaccessibleMembers = inaccessibleMembers;
            OverlongLines = overlongLines;
            Limit = limit;
            UndocumentedAbstractions = undocumentedAbstractions;
            UndocumentedImplementers = undocumentedImplementers;
        }

        /// <summary>Gets the types declared at the top level, in declaration order. RSA2008, RSA2009.</summary>
        public IReadOnlyList<MemberDeclarationSyntax> TopLevelTypes { get; }

        /// <summary>Gets every <c>#region</c> directive trivia found. RSA2010.</summary>
        public IReadOnlyList<SyntaxTrivia> Regions { get; }

        /// <summary>Gets members that can, but do not, declare accessibility. RSA2011.</summary>
        public IReadOnlyList<MemberDeclarationSyntax> InaccessibleMembers { get; }

        /// <summary>Gets lines longer than the configured limit. RSA2013.</summary>
        public IReadOnlyList<OverlongLine> OverlongLines { get; }

        /// <summary>Gets the configured <c>max_line_length</c>, or null when none is configured. RSA2013.</summary>
        public int? Limit { get; }

        /// <summary>Gets interfaces, abstract types, and their abstract/interface members that have no XML documentation summary. RSA2014.</summary>
        public IReadOnlyList<MemberDeclarationSyntax> UndocumentedAbstractions { get; }

        /// <summary>Gets members that implement an interface member or override an abstract member and have no <c>&lt;inheritdoc/&gt;</c>. RSA2015.</summary>
        public IReadOnlyList<MemberDeclarationSyntax> UndocumentedImplementers { get; }
    }
}
