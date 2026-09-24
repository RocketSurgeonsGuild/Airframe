using FluentAssertions;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Rocket.Surgery.Airframe.CodeFixes.Design;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

/// <summary>
/// Starts from one file that violates every RSA2XXX rule with a code fix, applies fixes
/// iteratively until none remain, and snapshots the result. This is the living answer to "what
/// does a fully compliant file look like" and a regression guard against any fix that only works
/// in isolation.
/// </summary>
/// <remarks>
/// RSA2008 (single type per file), RSA2009 (file name matches type) and RSA2012 (conditional
/// compilation must not span members) have no code fix — each is a structural decision only the
/// author can make — so <see cref="Messy"/> does not violate them; there is nothing to iterate on.
///
/// The rules cannot be fixed in one pass: <see cref="MemberOrderFix"/> declines to reorder while a
/// directive (region, <c>#if</c>, <c>#pragma</c>) sits anywhere in the type, because reordering
/// could move a member across it. RSA2010's region fix has to land first. Rather than hardcode
/// that ordering, each iteration re-analyzes the current text and applies whichever fix is
/// actually offered, so the loop discovers the dependency the same way an IDE would.
///
/// The loop does not assert which specific rule each iteration resolved. When a document carries
/// several diagnostics at once, <c>GeneratorTestContextBuilder</c> can pair a resolved fix's
/// <c>Diagnostic</c> with a <c>CodeActions</c> list that came from a different diagnostic entirely
/// — the same cross-attribution quirk <see cref="Rsa2011FixTests"/> notes for repeated diagnostics
/// of one rule, just triggered here by having many different rules violated at once (tracked in
/// https://github.com/RocketSurgeonsGuild/Airframe/issues/359). Trusting that label produced a
/// spurious failure during development even though the loop's actual behavior — apply whichever
/// text change is offered, re-analyze, repeat — was correct throughout, so this test asserts only
/// what the harness reports reliably: the diagnostic count each pass, and the final, verified text.
/// </remarks>
public class AllDesignRulesFixedTests
{
    private static readonly System.Type[] Analyzers =
    [
        typeof(Rsa2001), typeof(Rsa2002), typeof(Rsa2003), typeof(Rsa2004), typeof(Rsa2005),
        typeof(Rsa2006), typeof(Rsa2007), typeof(Rsa2010), typeof(Rsa2011), typeof(Rsa2013)
    ];

    private static readonly System.Type[] Fixes =
    [
        typeof(MemberOrderFix), typeof(Rsa2010Fix), typeof(Rsa2011Fix), typeof(Rsa2013Fix)
    ];

    [Fact]
    public async Task GivenEveryRuleViolated_WhenFixesAppliedIteratively_ThenResultIsClean()
    {
        // Given
        var source = Messy;

        // When. Apply one resolvable fix per pass and re-analyze, the way an IDE's "fix all in
        // file" applies one code action at a time rather than merging unrelated edits.
        const int maxIterations = 20;
        var iterationsRun = 0;

        for (var iteration = 0; iteration < maxIterations; iteration++)
        {
            iterationsRun++;

            var result = await GeneratorTestContextBuilder
               .Create()
               .WithAnalyzer<Rsa2001>()
               .WithAnalyzer<Rsa2002>()
               .WithAnalyzer<Rsa2003>()
               .WithAnalyzer<Rsa2004>()
               .WithAnalyzer<Rsa2005>()
               .WithAnalyzer<Rsa2006>()
               .WithAnalyzer<Rsa2007>()
               .WithAnalyzer<Rsa2010>()
               .WithAnalyzer<Rsa2011>()
               .WithAnalyzer<Rsa2013>()
               .WithCodeFix<MemberOrderFix>()
               .WithCodeFix<Rsa2010Fix>()
               .WithCodeFix<Rsa2011Fix>()
               .WithCodeFix<Rsa2013Fix>()
               .AddNormalizedSources(source)
               .AddGlobalOption("max_line_length", "80")
               .Build()
               .GenerateAsync();

            var remaining = Analyzers
               .SelectMany(analyzer => result.AnalyzerResults[analyzer].Diagnostics)
               .ToList();

            if (remaining.Count == 0)
            {
                break;
            }

            var resolvedFix = Fixes
               .Select(fix => result.CodeFixResults[fix])
               .SelectMany(fixResult => fixResult.ResolvedFixes)
               .FirstOrDefault(fix => fix.CodeActions.Any());

            resolvedFix
               .Should()
               .NotBeNull($"iteration {iteration} still has {remaining.Count} diagnostic(s) but none has an offered fix: {string.Join(", ", remaining.Select(d => d.Id))}");

            var codeAction = resolvedFix!.CodeActions[0];
            var textChange = codeAction.TextChanges.Single().Value;
            var originalText = await resolvedFix.Document.GetTextAsync();

            source = originalText.WithChanges(textChange).ToString();
        }

        // Then. The loop made real progress rather than converging in one lucky pass, and the
        // final file has nothing left to fix.
        iterationsRun.Should().BeGreaterThan(1);

        var finalResult = await GeneratorTestContextBuilder
           .Create()
           .WithAnalyzer<Rsa2001>()
           .WithAnalyzer<Rsa2002>()
           .WithAnalyzer<Rsa2003>()
           .WithAnalyzer<Rsa2004>()
           .WithAnalyzer<Rsa2005>()
           .WithAnalyzer<Rsa2006>()
           .WithAnalyzer<Rsa2007>()
           .WithAnalyzer<Rsa2010>()
           .WithAnalyzer<Rsa2011>()
           .WithAnalyzer<Rsa2013>()
           .AddNormalizedSources(source)
           .AddGlobalOption("max_line_length", "80")
           .GenerateAsync();

        finalResult
           .AnalyzerResults
           .Values
           .SelectMany(r => r.Diagnostics)
           .Should()
           .BeEmpty("every rule with a fix should be resolved by the loop above");

        await Verifier.Verify(source);
    }

    /// <summary>
    /// The full matrix: every access level RSA2004 orders (public, internal, protected internal,
    /// protected, private protected, private) on every kind that can actually declare each of
    /// them — field, event, property, indexer, method, delegate, nested type — plus a constructor,
    /// a destructor and one operator (operators are always <c>public static</c> in C#, so access
    /// can't vary there). Every RSA2001-2007 kind/access/static/const/readonly combination this
    /// file can show, it shows, then runs the whole thing through the fixers.
    ///
    /// Indexers can't be named apart the way other kinds can, so each access level's indexer takes
    /// a different parameter type instead (int, string, long, char, double, bool) purely so the
    /// six don't collide as overloads; the parameter type carries no meaning of its own.
    ///
    /// Region (RSA2010), missing accessibility (RSA2011, on the same private method that also
    /// carries the RSA2002 violation) and one overlong line (RSA2013) are still exercised exactly
    /// as before; none of the new members touch those three.
    /// </summary>
    // lang=csharp
    internal const string Messy =
        """
        namespace Sample
        {
            public class Example
            {
                private readonly int _value;

                public Example()
                {
                }

                ~Example()
                {
                }

                #region Helpers
                void Helper()
                {
                }
                #endregion

                public int Property { get; set; }

                internal int InternalField = 2;

                protected internal int ProtectedInternalField = 10;

                protected int ProtectedField = 9;

                private protected int PrivateProtectedField = 11;

                public int PublicField = 3;

                public static int StaticField = 4;

                public const int ConstField = 5;

                public static int AnotherStaticField = 6;

                public static readonly int StaticReadonlyField = 7;

                public event System.EventHandler Changed;

                internal event System.EventHandler ChangedInternal;

                protected internal event System.EventHandler ChangedProtectedInternal;

                protected event System.EventHandler ChangedProtected;

                private protected event System.EventHandler ChangedPrivateProtected;

                private event System.EventHandler ChangedPrivate;

                internal int InternalProperty { get; set; }

                protected internal int ProtectedInternalProperty { get; set; }

                protected int ProtectedProperty { get; set; }

                private protected int PrivateProtectedProperty { get; set; }

                private int PrivateProperty { get; set; }

                public int this[int index] => index;

                internal int this[string key] => 0;

                protected internal int this[long index] => 0;

                protected int this[char key] => 0;

                private protected int this[double index] => 0;

                private int this[bool flag] => 0;

                public void LongMethod(string firstParameter, string secondParameter, string thirdParameter)
                {
                }

                internal void MethodInternal()
                {
                }

                protected internal void MethodProtectedInternal()
                {
                }

                protected void MethodProtected()
                {
                }

                private protected void MethodPrivateProtected()
                {
                }

                public static Example operator +(Example left, Example right) => left;

                public delegate void Handler();

                internal delegate void HandlerInternal();

                protected internal delegate void HandlerProtectedInternal();

                protected delegate void HandlerProtected();

                private protected delegate void HandlerPrivateProtected();

                private delegate void HandlerPrivate();

                public class Nested
                {
                }

                internal class NestedInternal
                {
                }

                protected internal class NestedProtectedInternal
                {
                }

                protected class NestedProtected
                {
                }

                private protected class NestedPrivateProtected
                {
                }

                private class NestedPrivate
                {
                }
            }
        }
        """;
}
