using DiffEngine;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System;
using System.IO;
using VerifyTests;
using VerifyTests.DiffPlex;
using static VerifyXunit.Verifier;

namespace Rocket.Surgery.Airframe.Defaults.Tests;

internal static class ModuleInitializer
{
    [global::System.Runtime.CompilerServices.ModuleInitializer]

    public static void Initialize()
    {
        VerifyGeneratorTextContext.Initialize();
        VerifyDiffPlex.Initialize(OutputType.Minimal);
        DiffRunner.Disabled = true;

        DerivePathInfo(
            (sourceFile, projectDirectory, type, method) =>
            {
                var typeName = GetTypeName(type);

                var path = Path.Combine(Path.GetDirectoryName(sourceFile)!, "Verified");
                return new(path, typeName, method.Name);

                static string GetTypeName(Type type) => type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;
            });
    }
}