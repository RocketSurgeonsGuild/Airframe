using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using Rocket.Surgery.Nuke;
using Rocket.Surgery.Nuke.DotNetCore;
using System;
using System.IO;
using System.Linq;

/// <summary>
/// Defines a `dotnet test` test run with code coverage via coverlet
/// </summary>
public interface ICanTestWithDotNetCoreNoBuild : IHaveCollectCoverage,
                                                 IHaveBuildTarget,
                                                 ITriggerCodeCoverageReports,
                                                 IComprehendTests,
                                                 IHaveTestArtifacts,
                                                 IHaveGitVersion,
                                                 IHaveSolution,
                                                 IHaveConfiguration,
                                                 IHaveOutputLogs,
                                                 ICan
{
    /// <summary>
    /// dotnet test
    /// </summary>
    public Target CoreTest => _ => _
       .Description("Executes all the unit tests.")
       .After(Build)
       .OnlyWhenDynamic(() => TestsDirectory.GlobFiles("**/*.csproj").Count > 0)
       .WhenSkipped(DependencyBehavior.Execute)
       .Executes(
            () =>
            {
                FileSystemTasks.EnsureCleanDirectory(TestResultsDirectory);
                CoverageDirectory.GlobFiles("*.cobertura.xml", "*.opencover.xml", "*.json", "*.info")
                   .Where(x => Guid.TryParse(Path.GetFileName(x)?.Split('.')[0], out var _))
                   .ForEach(FileSystemTasks.DeleteFile);
            })
       .Executes(
            () =>
            {
                var runsettings = TestsDirectory / "coverlet.runsettings";
                if (!runsettings.FileExists())
                {
                    runsettings = NukeBuild.TemporaryDirectory / "default.runsettings";
                    if (!runsettings.FileExists())
                    {
                        using var tempFile = File.Open(runsettings, FileMode.CreateNew);
                        typeof(ICanTestWithDotNetCore)
                           .Assembly
                           .GetManifestResourceStream("Rocket.Surgery.Nuke.default.runsettings")!.CopyTo(tempFile);
                    }
                }

                MSBuildTasks.MSBuild(
                    settings =>
                        settings
                           .SetSolutionFile(Solution)
                           .SetConfiguration(global::Configuration.Debug)
                           .SetDefaultLoggers(LogsDirectory / "test.build.log")
                           .SetGitVersionEnvironment(GitVersion)
                           .SetAssemblyVersion(GitVersion.AssemblySemVer)
                           .SetPackageVersion(GitVersion.NuGetVersionV2));

                DotNetTasks.DotNetTest(
                    s => s.SetProjectFile(Solution)
                       .SetDefaultLoggers(LogsDirectory / "test.log")
                       .SetGitVersionEnvironment(GitVersion)
                       .EnableNoRestore()
                       .SetConfiguration(global::Configuration.Debug)
                       .EnableNoBuild()
                        // DeterministicSourcePaths being true breaks coverlet!
                       .SetProperty("DeterministicSourcePaths", "false")
                       .SetResultsDirectory(TestResultsDirectory)
                       .When(
                            !CollectCoverage,
                            x => x.SetProperty("CollectCoverage", "true")
                               .SetProperty("CoverageDirectory", CoverageDirectory)
                        )
                       .When(
                            CollectCoverage,
                            x => x
                               .SetProperty("CollectCoverage", "false")
                               .SetDataCollector("XPlat Code Coverage")
                               .SetSettingsFile(runsettings)
                        )
                );

                // Ensure anything that has been dropped in the test results from a collector is
                // into the coverage directory
                foreach (var file in TestResultsDirectory
                            .GlobFiles("**/*.cobertura.xml")
                            .Where(x => Guid.TryParse(Path.GetFileName(x.Parent), out var _))
                            .SelectMany(coverage => coverage.Parent.GlobFiles("*.*")))
                {
                    var folderName = Path.GetFileName(file.Parent);
                    var extensionPart = string.Join(".", Path.GetFileName(file).Split('.').Skip(1));
                    FileSystemTasks.CopyFile(
                        file,
                        CoverageDirectory / $"{folderName}.{extensionPart}",
                        FileExistsPolicy.OverwriteIfNewer
                    );
                }
            }
        );
}