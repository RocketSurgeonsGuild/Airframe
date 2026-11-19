using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

/// <summary>Runs the built test assemblies with the repository's Coverlet collector.</summary>
public interface ICanTestWithDotNetCoreNoBuild : IHaveBuildTarget,
                                                 IHaveConfiguration,
                                                 IHaveGitVersion,
                                                 IHaveOutputLogs,
                                                 ITriggerCodeCoverageReports,
                                                 ICan
{
    /// <summary>Runs tests without rebuilding the solution.</summary>
    [NonEntryTarget]
    public Target CoreTest => definition => definition
       .Description("Executes all the unit tests with code coverage.")
       .Unlisted()
       .After(Build)
       .TryDependentFor<IHaveTestTarget>(target => target.Test)
       .WhenSkipped(DependencyBehavior.Execute)
       .Net9MsBuildFix()
       .Executes(() =>
        {
            TestResultsDirectory.CreateOrCleanDirectory();
            DotNetTasks.DotNetTest(settings => settings
               .SetProcessWorkingDirectory(NukeBuild.RootDirectory)
               .SetProjectFile(Solution)
               .SetConfiguration(Configuration)
               .SetGitVersionEnvironment(GitVersion)
               .SetDefaultLoggers(LogsDirectory / "test.log")
               .EnableNoRestore()
               .EnableNoBuild()
               .SetProperty("DeterministicSourcePaths", "false")
               .SetDataCollector("XPlat Code Coverage")
               .SetResultsDirectory(TestResultsDirectory));
        });
}
