using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA2008"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa2008 : Rsa2000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA2008];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);

        foreach (var type in DocumentWalk.Of(context).TopLevelTypes)
        {
            // Types that share a name are one concept spelled several ways, such as IListener
            // beside IListener<T>, or the parts of a partial type. They belong in one file.
            if (names.Add(MemberRank.NameOf(type)) && names.Count > 1)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(RSA2008, MemberRank.LocationOf(type), MemberRank.NameOf(type)));
            }
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.CompilationUnit];
}
