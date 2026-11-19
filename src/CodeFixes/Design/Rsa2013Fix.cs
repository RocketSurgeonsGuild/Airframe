using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Text;
using Rocket.Surgery.Airframe.Analyzers;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

namespace Rocket.Surgery.Airframe.CodeFixes.Design;

/// <summary>Wraps RSA2013 lines to a stable layout, preserving their tokens and comments.</summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa2013Fix))]
public class Rsa2013Fix : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [RSA2013.Id];

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => FixAll;

    /// <inheritdoc/>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        // Compute the complete edit before offering it. Unbreakable literals and signatures
        // must not produce no-op actions that dotnet format will keep attempting.
        var changed = await WrapAsync(context.Document, context.Diagnostics, context.CancellationToken).ConfigureAwait(false);
        var before = await context.Document.GetTextAsync(context.CancellationToken).ConfigureAwait(false);
        var after = await changed.GetTextAsync(context.CancellationToken).ConfigureAwait(false);
        if (before.ContentEquals(after))
        {
            return;
        }

        context.RegisterCodeFix(CodeAction.Create(Title, _ => Task.FromResult(changed), Title), context.Diagnostics);
    }

    internal static async Task<Document> WrapAsync(Document document, ImmutableArray<Diagnostic> diagnostics, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null || diagnostics.IsEmpty)
        {
            return document;
        }

        var config = document.Project.AnalyzerOptions.AnalyzerConfigOptionsProvider.GetOptions(root.SyntaxTree);

        // RSA2013's location begins exactly at the margin. Some hosts supply analyzer options
        // only during analysis, so the diagnostic also carries the limit when options are absent.
        var limit = diagnostics.Min(diagnostic => diagnostic.Location.GetLineSpan().StartLinePosition.Character);
        if (config.TryGetValue("max_line_length", out var value) && (!int.TryParse(value, out limit) || limit <= 0))
        {
            return document;
        }

        var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
        var options = await GetFormattingOptionsAsync(document, text, config, cancellationToken).ConfigureAwait(false);
        var newLine = options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
        var operatorsAtEnd = config.TryGetValue("dotnet_style_operator_placement_when_wrapping", out var placement)
                         && placement.Split(':')[0].Trim() == "end_of_line";
        var lines = new HashSet<int>(diagnostics.Select(diagnostic => text.Lines.GetLineFromPosition(diagnostic.Location.SourceSpan.Start).LineNumber));
        var eligible = new SyntaxAnnotation();
        var wrapped = new SyntaxAnnotation();
        var selected = new SyntaxAnnotation();
        var tokens = root.DescendantTokens().Where(token => lines.Contains(text.Lines.GetLineFromPosition(token.SpanStart).LineNumber));
        root = root.ReplaceTokens(tokens, (original, rewritten) => rewritten.WithAdditionalAnnotations(eligible));
        var selectedTargets = lines
           .SelectMany(number => LineChopper.Targets(root, text, text.Lines[number], eligible, wrapped, operatorsAtEnd))
           .Distinct()
           .ToArray();
        if (selectedTargets.Length == 0)
        {
            return document;
        }

        var formattingSpans = selectedTargets.Select(LineChopper.FormattingSpan)
           .Select(span => TextSpan.FromBounds(text.Lines.GetLineFromPosition(span.Start).Start, span.End))
           .Distinct().ToArray();
        root = root.ReplaceTokens(selectedTargets, (original, rewritten) => rewritten.WithAdditionalAnnotations(selected));

        // Normalize the owning construct before measuring its indentation. A diagnostic on a
        // continuation line alone can otherwise retain the declaration's old indentation level.
        document = await Formatter.FormatAsync(document.WithSyntaxRoot(root), formattingSpans, options, cancellationToken).ConfigureAwait(false);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;
            text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var comments = root.DescendantTrivia(descendIntoTrivia: true).Where(CommentTrivia.IsCommentOrDoc).ToList();

            // Finish every selected boundary even if initial formatting shortened the line.
            // Moving a body brace alone must not turn a parameter chop into a partial edit.
            var targets = root.GetAnnotatedTokens(selected).Where(token => !token.HasAnnotation(wrapped))
               .Concat(LineLength.Measure(text, comments, limit)
                   .SelectMany(line => LineChopper.Targets(root, text, text.Lines.GetLineFromPosition(line.Span.Start), eligible, wrapped, operatorsAtEnd)))
               .Distinct()
               .ToArray();
            if (targets.Length == 0)
            {
                return document;
            }

            // Keep every existing trivia, including comma comments, block comments and directives.
            // Roslyn preserves continuation indentation, so seed each new line from the current
            // construct plus one configured indentation level before formatting the affected spans.
            var breaks = new SyntaxAnnotation();
            var existingLineStarts = LineChopper.ExistingLineStarts(targets, text, eligible);
            root = root.ReplaceTokens(targets.Concat(existingLineStarts).Distinct(), (original, rewritten) =>
            {
                var indent = SyntaxFactory.Whitespace(ContinuationIndent(text, LineChopper.IndentationAnchor(original), options));
                var trivia = rewritten.LeadingTrivia;
                if (targets.Contains(original))
                {
                    trivia = trivia.Insert(0, indent).Insert(0, SyntaxFactory.EndOfLine(newLine));
                }
                else
                {
                    // The first item may already be on a new line. Align it with the items
                    // being chopped, retaining all intervening comments and existing breaks.
                    var index = 0;
                    for (var i = 0; i < trivia.Count; i++)
                    {
                        if (trivia[i].IsKind(SyntaxKind.EndOfLineTrivia))
                        {
                            index = i + 1;
                        }
                    }

                    if (index < trivia.Count && trivia[index].IsKind(SyntaxKind.WhitespaceTrivia))
                    {
                        trivia = trivia.RemoveAt(index);
                    }

                    trivia = trivia.Insert(index, indent);
                }

                return rewritten.WithLeadingTrivia(trivia).WithAdditionalAnnotations(wrapped, breaks);
            });
            var spans = root.GetAnnotatedTokens(breaks)
               .Select(LineChopper.FormattingSpan);
            document = await Formatter.FormatAsync(document.WithSyntaxRoot(root), spans, options, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task<OptionSet> GetFormattingOptionsAsync(
        Document document,
        SourceText text,
        AnalyzerConfigOptions config,
        CancellationToken cancellationToken)
    {
        OptionSet options = await document.GetOptionsAsync(cancellationToken).ConfigureAwait(false);

        // The public Roslyn option adapter does not resolve indent_size=tab in every host.
        // Resolve that EditorConfig relationship explicitly rather than retaining its default 4.
        if (config.TryGetValue("tab_width", out var width) && int.TryParse(width, out var tabWidth) && tabWidth > 0)
        {
            options = options.WithChangedOption(FormattingOptions.TabSize, LanguageNames.CSharp, tabWidth);
        }

        if (config.TryGetValue("indent_size", out var size))
        {
            if (string.Equals(size, "tab", StringComparison.OrdinalIgnoreCase))
            {
                options = options.WithChangedOption(
                    FormattingOptions.IndentationSize,
                    LanguageNames.CSharp,
                    options.GetOption(FormattingOptions.TabSize, LanguageNames.CSharp));
            }
            else if (int.TryParse(size, out var indentationSize) && indentationSize > 0)
            {
                options = options.WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, indentationSize);
            }
        }

        // With no explicit EOL policy, preserve the document's convention on every platform.
        var newLine = config.TryGetValue("end_of_line", out var ending)
            ? ending.ToLowerInvariant() switch { "crlf" => "\r\n", "cr" => "\r", "lf" => "\n", _ => options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp) }
            : text.Lines.Where(line => line.EndIncludingLineBreak > line.End)
                  .Select(line => text.ToString(TextSpan.FromBounds(line.End, line.EndIncludingLineBreak)))
                  .FirstOrDefault() ?? options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
        options = options.WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, newLine);
        return options;
    }

    private static string ContinuationIndent(SourceText text, int position, OptionSet options)
    {
        var tabSize = options.GetOption(FormattingOptions.TabSize, LanguageNames.CSharp);
        var indentationSize = options.GetOption(FormattingOptions.IndentationSize, LanguageNames.CSharp);
        var line = text.Lines.GetLineFromPosition(position);
        var column = 0;
        for (var index = line.Start; index < line.End; index++)
        {
            if (text[index] == '\t')
            {
                column += tabSize - (column % tabSize);
            }
            else if (text[index] == ' ')
            {
                column++;
            }
            else
            {
                break;
            }
        }

        column += indentationSize;
        return options.GetOption(FormattingOptions.UseTabs, LanguageNames.CSharp)
            ? new string('\t', column / tabSize) + new string(' ', column % tabSize)
            : new string(' ', column);
    }

    private const string Title = "Chop line";

    // Per-document fixing avoids BatchFixer's overlapping edits when several long lines belong
    // to the same expression. The callback consumes only diagnostics supplied by the host, so
    // suppressed lines and documents outside the selected Fix All scope remain untouched.
    private static readonly FixAllProvider FixAll = FixAllProvider.Create(
        async (context, document, diagnostics) => await WrapAsync(document, diagnostics, context.CancellationToken).ConfigureAwait(false));
}
