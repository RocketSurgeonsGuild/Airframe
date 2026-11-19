using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit.Abstractions;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Build;

/// <summary>Exercises the cleanup profile through the SDK formatter with the current analyzer assemblies.</summary>
public class CleanupTests(ITestOutputHelper output)
{
    [Fact]
    public async Task GivenScopedCleanup_WhenVerifiedAndApplied_ThenVerificationWritesNothingAndCleanupConverges()
    {
        // Given. The path contains spaces to exercise argument handling, and a second dirty file
        // proves that --include has the same meaning for both native and Airframe passes.
        var directory = Path.Combine(PhysicalDirectory(new DirectoryInfo(Path.GetTempPath())), "airframe cleanup " + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var sdk = await RunAsync(directory, "--version");
            sdk.ExitCode.Should().Be(0, sdk.Output);
            var version = sdk.Output.Trim();
            var framework = "net" + version.Split('.')[0] + ".0";
            var project = Path.Combine(directory, "Consumer.csproj");
            await File.WriteAllTextAsync(Path.Combine(directory, "Directory.Build.props"), "<Project />");
            await File.WriteAllTextAsync(Path.Combine(directory, "Directory.Build.targets"), "<Project />");
            await File.WriteAllTextAsync(Path.Combine(directory, "global.json"), "{\"sdk\":{\"version\":\"" + version + "\"}}");
            await File.WriteAllTextAsync(Path.Combine(directory, "NuGet.Config"), "<configuration><packageSources><clear/></packageSources></configuration>");
            await File.WriteAllTextAsync(project, $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>{framework}</TargetFramework>
                    <NuGetAudit>false</NuGetAudit>
                    <GenerateDocumentationFile>true</GenerateDocumentationFile>
                  </PropertyGroup>
                  <Import Project="{SecurityElement.Escape(Path.Combine(RepositoryDirectory(), ".build", "Cleanup.targets"))}" Condition="'$(AirframeCleanup)' == 'true'" />
                </Project>
                """);
            await File.WriteAllTextAsync(Path.Combine(directory, ".editorconfig"), """
                root = true
                [*.cs]
                dotnet_analyzer_diagnostic.severity = none
                dotnet_diagnostic.IDE0001.severity = suggestion
                dotnet_diagnostic.IDE0002.severity = suggestion
                dotnet_diagnostic.IDE0005.severity = suggestion
                dotnet_diagnostic.IDE0007.severity = suggestion
                dotnet_diagnostic.IDE0090.severity = suggestion
                dotnet_diagnostic.RSA2016.severity = warning
                csharp_style_var_elsewhere = true:suggestion
                csharp_style_var_when_type_is_apparent = true:suggestion
                csharp_style_var_for_built_in_types = true:suggestion
                csharp_style_implicit_object_creation_when_type_is_apparent = true:suggestion
                """);
            const string source = """
                using N;
                using System.Collections;
                using Item = N.Item;
                class Consumer
                {
                    public Item Create() => new Item();
                    public object CreateObject() => new object();
                    public object Read()
                    {
                        object value = new object();
                        return value;
                    }
                }
                namespace N { public class Item { } }
                """;
            const string untouched = "using N; using Item = N.Item; class Untouched { public Item Create() => new Item(); }";
            var selectedFile = Path.Combine(directory, "Consumer.cs");
            var otherFile = Path.Combine(directory, "Untouched.cs");
            await File.WriteAllTextAsync(selectedFile, source);
            await File.WriteAllTextAsync(otherFile, untouched);
            var restore = await RunAsync(directory, "restore", project);
            restore.ExitCode.Should().Be(0, restore.Output);

            // When, Then. A failed verification must leave both files byte-for-byte intact.
            (await RunProfileAsync(directory, project, verify: true)).Should().NotBe(0);
            (await File.ReadAllTextAsync(selectedFile)).Should().Be(source);
            (await File.ReadAllTextAsync(otherFile)).Should().Be(untouched);

            // Isolate the native pass: after alias cleanup, suggestion-level native fixes alone
            // must still fail verification without being applied by the verify-only command.
            var analyzer = CleanupCommands.Create(project, ["Consumer.cs"], verify: false).First();
            (await RunAsync(directory, analyzer)).ExitCode.Should().Be(0);
            var analyzerClean = await File.ReadAllTextAsync(selectedFile);
            analyzerClean.Should().NotContain("using Item =");
            (await RunProfileAsync(directory, project, verify: true)).Should().NotBe(0);
            (await File.ReadAllTextAsync(selectedFile)).Should().Be(analyzerClean);

            (await RunProfileAsync(directory, project, verify: false)).Should().Be(0);
            var cleaned = await File.ReadAllTextAsync(selectedFile);
            cleaned.Should().NotContain("using Item =").And.NotContain("using System.Collections;").And.Contain("var value").And.Contain("new()");
            (await File.ReadAllTextAsync(otherFile)).Should().Be(untouched);

            (await RunProfileAsync(directory, project, verify: true)).Should().Be(0);
            (await File.ReadAllTextAsync(selectedFile)).Should().Be(cleaned);
            (await File.ReadAllTextAsync(otherFile)).Should().Be(untouched);
            var build = await RunAsync(directory, "build", project, "--no-restore", "-p:UseSharedCompilation=false", "-m:1");
            build.ExitCode.Should().Be(0, build.Output);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private async Task<int> RunProfileAsync(string directory, string project, bool verify)
    {
        foreach (var command in CleanupCommands.Create(project, ["Consumer.cs"], verify))
        {
            var result = await RunAsync(directory, [.. command, "--verbosity", "diagnostic"]);
            if (result.ExitCode != 0)
            {
                return result.ExitCode;
            }
        }

        return 0;
    }

    // The SDK resolves include filters from the physical working directory. Match that path
    // when macOS exposes its temporary directory through /var -> /private/var.
    private static string PhysicalDirectory(DirectoryInfo directory) => directory.ResolveLinkTarget(returnFinalTarget: true) is { } target
        ? target.FullName
        : directory.Parent is { } parent ? Path.Combine(PhysicalDirectory(parent), directory.Name) : directory.FullName;

    private async Task<(int ExitCode, string Output)> RunAsync(string directory, params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = directory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        foreach (var argument in arguments)
        {
            start.ArgumentList.Add(argument);
        }

        start.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        start.Environment["MSBUILDDISABLENODEREUSE"] = "1";
        start.Environment["AirframeCleanup"] = "true";
#if DEBUG
        start.Environment["Configuration"] = "Debug";
#else
        start.Environment["Configuration"] = "Release";
#endif
        using var process = Process.Start(start)!;
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            throw;
        }

        var log = await stdout + await stderr;
        output.WriteLine("dotnet " + string.Join(" ", arguments) + Environment.NewLine + log);
        return (process.ExitCode, log);
    }

    private static string RepositoryDirectory([CallerFilePath] string file = "") =>
        Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, "..", "..", ".."));
}
