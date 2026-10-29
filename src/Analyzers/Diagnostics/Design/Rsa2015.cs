using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA2015"/>.
/// </summary>
/// <remarks>
/// Reports on a member that either implements an interface member or overrides an abstract
/// member, whichever type directly declares that member; inheriting an implementation from a base
/// class does not itself require an <c>&lt;inheritdoc/&gt;</c> at the derived class. A member that
/// is itself abstract, re-declaring the contract rather than fulfilling it, is excluded; RSA2014
/// owns documenting that one instead.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa2015 : Rsa2000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA2015];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        foreach (var member in DocumentWalk.Of(context).UndocumentedImplementers)
        {
            context.ReportDiagnostic(Diagnostic.Create(RSA2015, MemberRank.LocationOf(member), MemberRank.NameOf(member)));
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.CompilationUnit];
}
