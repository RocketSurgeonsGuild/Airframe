using DiffEngine;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyTests.DiffPlex;
using static VerifyXunit.Verifier;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyGeneratorTextContext.Initialize(DiagnosticSeverity.Error);
        VerifyDiffPlex.Initialize(OutputType.Minimal);
        DiffRunner.Disabled = true;

        // Normalize line endings across platforms (raw string literals embed Environment.NewLine)
        VerifierSettings.AddScrubber(sb => sb.Replace("\r\n", "\n"));

        DerivePathInfo((sourceFile, projectDirectory, type, method) =>
        {
            var typeName = GetTypeName(type);

            var path = Path.Combine(Path.GetDirectoryName(sourceFile)!, "Verified");
            return new(path, typeName, method.Name);

            static string GetTypeName(Type type) => type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;
        });
    }
}