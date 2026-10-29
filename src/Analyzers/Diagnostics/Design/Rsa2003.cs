using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Rocket.Surgery.Airframe.Analyzers.Descriptions;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Represents a diagnostic for <see cref="Descriptions.RSA2003"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Rsa2003 : Rsa2000
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RSA2003];

    /// <inheritdoc/>
    protected override void Analyze(SyntaxNodeAnalysisContext context) =>
        MemberOrderWalk.Report(context, RSA2003, RankComponent.Kind);
}
