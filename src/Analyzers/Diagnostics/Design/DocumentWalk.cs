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

        var regions = compilationUnit
           .DescendantTrivia(descendIntoTrivia: true)
           .Where(trivia => trivia.IsKind(SyntaxKind.RegionDirectiveTrivia))
           .ToList();

        var inaccessibleMembers = compilationUnit
           .DescendantNodes()
           .OfType<MemberDeclarationSyntax>()
           .Where(Rsa2011.CanDeclareAccessibility)
           .Where(member => !HasAccessibility(member))
           .ToList();

        List<OverlongLine> overlongLines = [];
        int? limit = null;

        if (TryGetLimit(context.Options.AnalyzerConfigOptionsProvider.GetOptions(compilationUnit.SyntaxTree), out var configuredLimit))
        {
            limit = configuredLimit;

            foreach (var line in compilationUnit.SyntaxTree.GetText(context.CancellationToken).Lines)
            {
                var length = line.End - line.Start;
                if (length <= configuredLimit)
                {
                    continue;
                }

                // Squiggle only the part past the margin, so the reader sees what has to go.
                overlongLines.Add(new OverlongLine(TextSpan.FromBounds(line.Start + configuredLimit, line.End), length));
            }
        }

        var undocumentedAbstractions = compilationUnit
           .DescendantNodes()
           .OfType<MemberDeclarationSyntax>()
           .Where(Abstractions.IsAbstraction)
           .Where(member => !Abstractions.HasSummary(member))
           .ToList();

        var undocumentedImplementers = Abstractions
           .FindImplementers(compilationUnit, context.SemanticModel)
           .Where(member => !Abstractions.HasInheritdoc(member))
           .ToList();

        return new Result(topLevelTypes, regions, inaccessibleMembers, overlongLines, limit, undocumentedAbstractions, undocumentedImplementers);
    }

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
