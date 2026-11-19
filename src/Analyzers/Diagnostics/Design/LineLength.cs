using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>Measures code lines identically for RSA2013 and its wrapping fix.</summary>
internal static class LineLength
{
    internal static IReadOnlyList<DocumentWalk.OverlongLine> Measure(SourceText text, List<SyntaxTrivia> commentTrivia, int limit)
    {
        List<DocumentWalk.OverlongLine> overlongLines = [];

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
            if (length <= limit)
            {
                continue;
            }

            // Squiggle only the part past the margin, so the reader sees what has to go. The
            // squiggle ends at codeEnd, not the raw line end, so a trailing comment run is
            // never underlined alongside the code that pushed the line over the margin.
            overlongLines.Add(new DocumentWalk.OverlongLine(TextSpan.FromBounds(line.Start + limit, codeEnd), length));
        }

        return overlongLines;
    }
}
