using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common.CI.GitHubActions;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.DotNetCore;
using Rocket.Surgery.Nuke.GithubActions;
using System;
using static GitHubActionsTasks;

[GitHubActionsSteps(
    "ci",
    GitHubActionsImage.MacOsLatest,
    AutoGenerate = true,
    On = [RocketSurgeonGitHubActionsTrigger.Push],
    OnPushTags = ["v*"],
    OnPushBranches = ["master", "next", "feature/*"],
    OnPullRequestBranches = ["master", "next"],
    InvokedTargets = [nameof(GitHubActions)],
    NonEntryTargets =
    [
        nameof(ICIEnvironment.CIEnvironment),
        nameof(ITriggerCodeCoverageReports.TriggerCodeCoverageReports),
        nameof(ITriggerCodeCoverageReports.GenerateCodeCoverageReportCobertura),
        nameof(IGenerateCodeCoverageBadges.GenerateCodeCoverageBadges),
        nameof(IGenerateCodeCoverageReport.GenerateCodeCoverageReport),
        nameof(IGenerateCodeCoverageSummary.GenerateCodeCoverageSummary),
        nameof(Default)
    ],
    ExcludedTargets = [nameof(ICanClean.Clean), nameof(ICanRestoreWithDotNetCore.DotnetToolRestore)],
    Enhancements = [nameof(Middleware)]
)]
[PrintBuildVersion, PrintCIEnvironment, UploadLogs]
[LocalBuildConventions]
[ContinuousIntegrationConventions]
public partial class AirframeBuild
{
    public Target GitHubActions => definition => definition
       .OnlyWhenStatic(IsRunningOnGitHubActions)
       .DependsOn(Workloads)
       .DependsOn(Restore)
       .DependsOn(Build)
       .DependsOn(Test)
       .DependsOn(Pack)
       .Executes();

    public Target Workloads => definition => definition
       .Before(Restore)
       .Executes(() => DotNetTasks.DotNetWorkloadInstall(configurator => configurator.AddWorkloadId("ios", "maccatalyst")));

    public static RocketSurgeonGitHubActionsConfiguration Middleware(RocketSurgeonGitHubActionsConfiguration configuration)
    {
        var buildJob = configuration.Jobs.Cast<RocketSurgeonsGithubActionsJob>().First(z => z.Name.Equals("build", StringComparison.OrdinalIgnoreCase));
        var checkoutStep = buildJob.Steps.OfType<CheckoutStep>().Single();
        // For fetch all
        checkoutStep.FetchDepth = 0;
        buildJob.Steps.InsertRange(
            buildJob.Steps.IndexOf(checkoutStep) + 1,
            new BaseGitHubActionsStep[]
            {
                new RunStep("Fetch all history for all tags and branches")
                {
                    Run = "git fetch --prune"
                },
                new SetupDotNetStep("Use .NET 8 SDK")
                {
                    DotNetVersion = "8.0.101"
                }
            }
        );

        buildJob.Steps.Add(
            new UsingStep("Publish Coverage")
            {
                Uses = "codecov/codecov-action@v1",
                With = new Dictionary<string, string>
                {
                    ["name"] = "actions-${{ matrix.os }}",
                }
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish logs")
            {
                Name = "logs",
                Path = "artifacts/logs/",
                If = "always()"
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish coverage data")
            {
                Name = "coverage",
                Path = "coverage/",
                If = "always()"
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish test data")
            {
                Name = "test data",
                Path = "artifacts/test/",
                If = "always()"
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish NuGet Packages")
            {
                Name = "nuget",
                Path = "artifacts/nuget/",
                If = "always()"
            }
        );

        return configuration;
    }
}

public static class GitHubActionsTasks
{
    public static Func<bool> IsRunningOnGitHubActions => ()
        => NukeBuild.Host is GitHubActions || Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == true.ToString();
}