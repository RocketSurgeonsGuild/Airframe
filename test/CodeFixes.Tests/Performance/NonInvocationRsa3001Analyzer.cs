using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Performance;

/// <summary>
/// Test-only analyzer that reports <see cref="Descriptions.RSA3001"/> on a class identifier, a node
/// <c>Rsa3001</c> never reports on, so the code fix can be exercised against a non-invocation shape.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NonInvocationRsa3001Analyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [Descriptions.RSA3001];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            c => c.ReportDiagnostic(Diagnostic.Create(Descriptions.RSA3001, ( (ClassDeclarationSyntax)c.Node ).Identifier.GetLocation())),
            SyntaxKind.ClassDeclaration);
    }
}
