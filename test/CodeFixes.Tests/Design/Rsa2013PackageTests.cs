using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;
using Xunit.Abstractions;

namespace Rocket.Surgery.Airframe.CodeFixes.Tests.Design;

/// <summary>Exercises discovery and Fix All through the shipped package and the SDK CLI.</summary>
public class Rsa2013PackageTests(ITestOutputHelper output)
{
    [Fact]
    public async Task GivenPackedAnalyzer_WhenDotnetFormatRunsOnce_ThenConsumersCompileAndASecondRunChangesNothing()
    {
        // Given. A unique version and package cache prevent a previously packed DLL from passing.
        var repository = RepositoryDirectory();
        var temporary = Path.Combine(Path.GetTempPath(), "airframe-format-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temporary);
        try
        {
            var sdkVersion = (await RunAsync(repository, "--version")).Trim();
            var targetFramework = "net" + sdkVersion.Split('.')[0] + ".0";
            await File.WriteAllTextAsync(Path.Combine(temporary, "global.json"),
                "{\"sdk\":{\"version\":\"" + sdkVersion + "\"}}");
            var feed = Path.Combine(temporary, "feed");
            var version = "0.0.0-test-" + Guid.NewGuid().ToString("N");
#if DEBUG
            const string configuration = "Debug";
#else
            const string configuration = "Release";
#endif
            await RunAsync(repository, "pack", Path.Combine(repository, "src", "CodeAnalysis", "CodeAnalysis.csproj"),
                "--no-build", "--no-restore", "--configuration", configuration, "--output", feed,
                "-p:PackageVersion=" + version, "-p:PackageIcon=");
            Directory.GetFiles(feed, "*.nupkg").Should().ContainSingle();

            foreach (var useTabs in new[] { false, true })
            {
                var directory = Path.Combine(temporary, useTabs ? "tabs" : "spaces");
                Directory.CreateDirectory(directory);
                var newLine = useTabs ? "\r\n" : "\n";
                await File.WriteAllTextAsync(Path.Combine(directory, "NuGet.Config"),
                    $"<configuration><packageSources><clear/><add key=\"local\" value=\"{SecurityElement.Escape(feed)}\"/></packageSources></configuration>");
                await File.WriteAllTextAsync(Path.Combine(directory, "Consumer.csproj"), $"""
                    <Project Sdk="Microsoft.NET.Sdk">
                      <PropertyGroup>
                        <TargetFramework>{targetFramework}</TargetFramework>
                        <RestorePackagesPath>packages</RestorePackagesPath>
                        <NuGetAudit>false</NuGetAudit>
                      </PropertyGroup>
                      <ItemGroup>
                        <PackageReference Include="Rocket.Surgery.Airframe.CodeAnalysis" Version="{version}" PrivateAssets="all" />
                      </ItemGroup>
                    </Project>
                    """);
                await File.WriteAllTextAsync(Path.Combine(directory, ".editorconfig"),
                    "root = true\n[*.cs]\ndotnet_analyzer_diagnostic.severity = none\ndotnet_diagnostic.RSA2013.severity = warning\nmax_line_length = 60\n"
                    + (useTabs ? "indent_style = tab\nindent_size = tab\ntab_width = 4\nend_of_line = crlf\n" : "indent_style = space\nindent_size = 2\nend_of_line = lf\n"));
                var cases = new Rsa2013WrappingTests.WrappingCases().ToArray();
                var sources = cases.Select((data, index) =>
                    ("using System;\nusing System.Linq;\nclass Example" + index + "\n{\n" + data[1] + "\n}\n").ReplaceLineEndings(newLine)).ToArray();
                for (var index = 0; index < sources.Length; index++)
                {
                    await File.WriteAllTextAsync(Path.Combine(directory, $"Example{index}.cs"), sources[index]);
                }

                await RunAsync(directory, "restore", "Consumer.csproj", "--configfile", "NuGet.Config");
                var initialBuild = await RunAsync(directory, "build", "Consumer.csproj", "--no-restore", "-p:UseSharedCompilation=false", "-m:1");
                initialBuild.Should().Contain("RSA2013", "the installed package must actually report the fixture's long lines");

                // When. One CLI invocation must finish every nested and overlapping wrap.
                await RunAsync(directory, "format", "Consumer.csproj", "--no-restore", "--diagnostics", "RSA2013");

                // Then. Compilation with RSA2013 promoted to an error proves no long lines remain.
                await RunAsync(directory, "build", "Consumer.csproj", "--no-restore", "-p:WarningsAsErrors=RSA2013", "-p:UseSharedCompilation=false", "-m:1");
                var fixedSources = new string[sources.Length];
                for (var index = 0; index < sources.Length; index++)
                {
                    fixedSources[index] = await File.ReadAllTextAsync(Path.Combine(directory, $"Example{index}.cs"));
                    var original = CSharpSyntaxTree.ParseText(sources[index]).GetRoot();
                    var changed = CSharpSyntaxTree.ParseText(fixedSources[index]).GetRoot();
                    changed.DescendantTokens().Select(token => (token.RawKind, token.Text)).Should()
                       .Equal(original.DescendantTokens().Select(token => (token.RawKind, token.Text)));
                    changed.DescendantTrivia().Where(CommentTrivia.IsCommentOrDoc).Select(trivia => trivia.ToFullString()).Should()
                       .Equal(original.DescendantTrivia().Where(CommentTrivia.IsCommentOrDoc).Select(trivia => trivia.ToFullString()));
                    fixedSources[index].Replace(newLine, "").Should().NotContain("\r").And.NotContain("\n");
                    if ((string)cases[index][0] is "method parameters" or "partially wrapped parameters")
                    {
                        var indent = useTabs ? "\t\t" : "    ";
                        fixedSources[index].Should().Contain("THing(" + newLine
                            + indent + "int longthing," + newLine
                            + indent + "int shortThing," + newLine
                            + indent + "int bigThing," + newLine
                            + indent + "int littleThing)");
                    }
                }

                fixedSources[0].Should().Contain(newLine + (useTabs ? "\t\t" : "    ") + "Enumerable.Range");
                await RunAsync(directory, "format", "Consumer.csproj", "--no-restore", "--diagnostics", "RSA2013", "--verify-no-changes");
                for (var index = 0; index < sources.Length; index++)
                {
                    (await File.ReadAllTextAsync(Path.Combine(directory, $"Example{index}.cs"))).Should().Be(fixedSources[index]);
                }
            }
        }
        finally
        {
            Directory.Delete(temporary, recursive: true);
        }
    }

    private async Task<string> RunAsync(string directory, params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = directory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var argument in arguments)
        {
            start.ArgumentList.Add(argument);
        }

        start.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        start.Environment["MSBUILDDISABLENODEREUSE"] = "1";
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
        process.ExitCode.Should().Be(0, log);
        return log;
    }

    private static string RepositoryDirectory([CallerFilePath] string file = "") =>
        Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, "..", "..", ".."));
}
