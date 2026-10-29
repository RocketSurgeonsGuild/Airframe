using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA2014"/>.
/// </summary>
/// <remarks>
/// Reports on the interface or abstract type declaration itself, and on every member it declares
/// without a body: an interface member with no default implementation (a <c>static abstract</c>
/// member included, since it has no body either), or a member marked <c>abstract</c> in an
/// abstract class. A default interface member and a concrete member of an abstract class are
/// excluded, since neither is part of the contract an implementer has to fulfil sight unseen.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa2014 : Rsa2000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA2014];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        foreach (var member in DocumentWalk.Of(context).UndocumentedAbstractions)
        {
            context.ReportDiagnostic(Diagnostic.Create(RSA2014, MemberRank.LocationOf(member), MemberRank.NameOf(member)));
        }
    }

    /// <inheritdoc/>
    protected override SyntaxKind[] GetSyntaxKind() => [SyntaxKind.CompilationUnit];
}
