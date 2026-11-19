using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

public partial class AirframeBuild
{
    /// <summary>Applies the repository's selected native and Airframe code fixes.</summary>
    public Target Cleanup => definition => definition
       .Description("Applies the selected code cleanup rules, including suggestions.")
       .DependsOn(BuildCleanupAnalyzers)
       .Executes(() => RunCleanup(verify: false));

    /// <summary>Checks the same cleanup selection without changing source files.</summary>
    public Target VerifyCleanup => definition => definition
       .Description("Verifies the selected cleanup rules without changing source files.")
       .DependsOn(BuildCleanupAnalyzers)
       .Executes(() => RunCleanup(verify: true));

    private Target BuildCleanupAnalyzers => definition => definition
       .Unlisted()
       .DependsOn(Restore)
       .Executes(() => DotNetTasks.DotNetBuild(settings => settings
           .SetProjectFile(RootDirectory / "src" / "CodeFixes" / "CodeFixes.csproj")
           .SetConfiguration(Configuration)
           .EnableNoRestore()));

    private void RunCleanup(bool verify)
    {
        var project = CleanupProject == null
            ? Solution.Path.ToString()
            : Path.GetFullPath(CleanupProject, RootDirectory);
        Assert.FileExists(project);
        Assert.True(CleanupFiles.All(file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)), "--cleanup-files accepts C# files only.");
        // The SDK's include filter expects paths relative to its working directory.
        var files = CleanupFiles.Select(file => Path.GetRelativePath(RootDirectory, Path.GetFullPath(file, RootDirectory))).ToArray();
        foreach (var file in files)
        {
            Assert.FileExists(Path.Combine(RootDirectory, file));
        }

        foreach (var command in CleanupCommands.Create(project, files, verify))
        {
            // The pinned ICanDotNetFormat exposes severity but no diagnostic-filter or verify-only
            // hook. Reuse NUKE's formatter tool while keeping its existing lint target unchanged.
            DotNetTasks.DotNetFormat(settings => settings
               .SetProcessWorkingDirectory(RootDirectory)
               .SetProcessEnvironmentVariable("AirframeCleanup", "true")
               .SetProcessEnvironmentVariable("Configuration", Configuration.ToString())
               .SetProcessArgumentConfigurator(_ =>
                {
                    var arguments = new Arguments();
                    foreach (var argument in command)
                    {
                        arguments.Add("{value}", argument);
                    }

                    return arguments;
                }));
        }
    }

    [Parameter("Project or solution for cleanup; defaults to the selected solution")]
    private readonly string? CleanupProject;

    [Parameter("Semicolon-separated C# files relative to the repository root; defaults to every source file", Separator = ";")]
    private readonly string[] CleanupFiles = [];
}
