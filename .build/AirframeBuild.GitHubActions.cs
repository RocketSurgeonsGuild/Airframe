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
    GitHubActionsImage.WindowsLatest,
    AutoGenerate = true,
    On = [RocketSurgeonGitHubActionsTrigger.Push],
    OnPushTags = ["v*"],
    OnPushBranches = ["main", "next", "feature/*"],
    OnPullRequestBranches = ["main", "next"],
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
    /// <summary>
    ///     The default pipeline. Deliberately does not depend on <see cref="Workloads" />: every project in
    ///     <c>Airframe.sln</c> is netstandard2.0 or net9.0, so this leg needs no workload installed. The only
    ///     project that does — <c>src/Apple</c>, which is net9.0-ios — lives in <c>Apple.sln</c> and is built
    ///     by the <c>build-apple</c> job instead.
    /// </summary>
    public Target GitHubActions => definition => definition
       .OnlyWhenStatic(IsRunningOnGitHubActions)
       .DependsOn(Restore)
       .DependsOn(Build)
       .DependsOn(Test)
       .DependsOn(Pack)
       .Executes();

    /// <summary>
    ///     Installs the ios workload. Only the <c>build-apple</c> job invokes this.
    /// </summary>
    public Target Workloads => definition => definition
       .Before(Restore)
       .Executes(
            () => DotNetTasks.DotNetWorkloadInstall(
                configurator => configurator
                               .AddWorkloadId("ios")
                                // the manifests are pinned by the sdk version, so updating them on every run
                                // costs minutes and changes nothing
                               .SetSkipManifestUpdate(true)
            )
        );

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

        configuration.AddJob(CreateChangesJob());
        configuration.AddJob(CreateAppleJob());

        return configuration;
    }

    /// <summary>
    ///     Decides whether the ios leg has to run at all. GitHub has no job level <c>paths:</c> filter, so the
    ///     decision is computed once here and consumed by <see cref="CreateAppleJob" />. Costs a few seconds on
    ///     ubuntu and runs in parallel with <c>build</c>, so it never lands on the critical path.
    /// </summary>
    private static RocketSurgeonsGithubActionsJob CreateChangesJob()
    {
        var job = new RocketSurgeonsGithubActionsJob(ChangesJobName)
        {
            RunsOn = ["ubuntu-latest"],
            Outputs = { new GitHubActionsStepOutput(AppleFilterStepId, AppleFilterName) }
        };

        job.Steps.Add(new CheckoutStep("Checkout"));
        job.Steps.Add(
            new UsingStep("Detect apple changes")
            {
                Id = AppleFilterStepId,
                Uses = "dorny/paths-filter@v3",
                // the filter lives in a file because `with` is emitted as flat `key: value`,
                // which cannot carry the multi line yaml this action otherwise expects
                With = { ["filters"] = AppleFilterFile }
            }
        );

        return job;
    }

    /// <summary>
    ///     Builds and packs <c>Rocket.Surgery.Airframe.Apple</c>. This is the only job that installs a workload.
    ///     It is skipped unless something it depends on changed, or the run is a release. A job skipped by `if:`
    ///     reports as successful, so this may remain a required status check without ever blocking a pull request.
    /// </summary>
    private static RocketSurgeonsGithubActionsJob CreateAppleJob()
    {
        var job = new RocketSurgeonsGithubActionsJob("build-apple")
        {
            RunsOn = ["windows-latest"],
            Needs = { ChangesJobName },
            // wrapped in ${{ }} because the writer emits `if:` unquoted, and a bare leading `!` is a yaml tag.
            // !cancelled() rather than a plain condition so a release still builds apple even if the
            // `changes` job failed — otherwise a broken filter would silently ship a release without the package.
            If = "${{ !cancelled() && ("
               + $"needs.{ChangesJobName}.outputs.{AppleFilterOutput} == 'true'"
               + " || startsWith(github.ref, 'refs/tags/v')"
               + " || github.ref == 'refs/heads/main'"
               + ") }}"
        };

        // gitversion has to see the full history or the apple package version drifts from the rest of the suite
        job.Steps.Add(new CheckoutStep("Checkout") { FetchDepth = 0 });
        job.Steps.Add(new RunStep("Fetch all history for all tags and branches") { Run = "git fetch --prune" });
        job.Steps.Add(new SetupDotNetStep("Use .NET 8 SDK") { DotNetVersion = "8.0.101" });

        // NOTE: no workload pack cache here yet. `with` is emitted as flat `key: value`, so the multi path
        // form actions/cache wants cannot be generated, and restoring into "C:\Program Files\dotnet" needs an
        // elevated runner. Gating this job is what takes the workload off the critical path; caching only
        // shortens the rare run that still pays it. Measure this job first, then decide if it is worth it.
        job.Steps.Add(new RunStep("dotnet tool restore") { Run = "dotnet tool restore" });
        job.Steps.Add(new RunStep("Workloads") { Run = "dotnet nuke Workloads --skip" });

        // --solution overrides the `Solution` parameter pinned in .nuke/parameters.json, pointing every
        // lifecycle target at Apple.sln instead of Airframe.sln. there is no Test step: apple has no tests.
        foreach (var target in new[] { nameof(Restore), nameof(Build), nameof(Pack) })
        {
            job.Steps.Add(new RunStep(target) { Run = $"dotnet nuke {target} --skip --solution {AppleSolution}" });
        }

        job.Steps.Add(
            new UploadArtifactStep("Publish Apple NuGet Package")
            {
                // publish-nuget.yml downloads each artifact by name, and upload-artifact rejects a second
                // upload under an existing name, so this cannot be folded into the `nuget` artifact.
                // scoped to the Apple package on purpose: Apple.sln also contains Core, and the `build`
                // job already packs and uploads that one.
                Name = "nuget-apple",
                Path = "artifacts/nuget/Rocket.Surgery.Airframe.Apple.*",
                If = "always()"
            }
        );

        return job;
    }

    private const string AppleSolution = "Apple.sln";
    private const string ChangesJobName = "changes";
    private const string AppleFilterStepId = "filter";
    private const string AppleFilterName = "apple";
    private const string AppleFilterFile = ".github/apple-filter.yml";

    /// <summary>
    ///     The key <see cref="RocketSurgeonsGithubActionsJobBase.Outputs" /> generates for the apple filter:
    ///     camelize(stepId + pascalize(outputName)).
    /// </summary>
    private const string AppleFilterOutput = AppleFilterStepId + "Apple";
}

public static class GitHubActionsTasks
{
    public static Func<bool> IsRunningOnGitHubActions => ()
        => NukeBuild.Host is GitHubActions || Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == true.ToString();
}