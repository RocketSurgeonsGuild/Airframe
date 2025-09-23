using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Rocket.Surgery.Nuke.DotNetCore;

[PublicAPI]
[UnsetVisualStudioEnvironmentVariables]
[PackageIcon("https://raw.githubusercontent.com/RocketSurgeonsGuild/graphics/master/png/social-square-thrust-rounded.png")]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
[EnsureGitHooks(GitHook.CommitMsg)]
public partial class AirframeBuild : NukeBuild,
                                     ICanRestoreWithDotNetCore,
                                     ICanBuildWithDotNetCore,
                                     ICanTestWithDotNetCore,
                                     ICanPackWithDotNetCore,
                                     ICanClean,
                                     ICanDotNetFormat,
                                     IHavePublicApis,
                                     ICanUpdateReadme,
                                     IGenerateCodeCoverageReport,
                                     IGenerateCodeCoverageSummary,
                                     IGenerateCodeCoverageBadges,
                                     ICanRegenerateBuildConfiguration,
                                     IHaveConfiguration<Configuration>

{
    /// <summary>
    ///     Support plugins are available for:
    ///     - JetBrains ReSharper        https://nuke.build/resharper
    ///     - JetBrains Rider            https://nuke.build/rider
    ///     - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///     - Microsoft VSCode           https://nuke.build/vscode
    /// </summary>
    public static int Main() => Execute<AirframeBuild>(build => build.Default);

    public Target Default => definition => definition
       .DependsOn(Restore)
       .DependsOn(Build)
       .DependsOn(Test)
       .DependsOn(Pack);

    public Target Clean => definition => definition.Inherit<ICanClean>(canClean => canClean.Clean);
    public Target Restore => definition => definition.Inherit<ICanRestoreWithDotNetCore>(dotNetCore => dotNetCore.DotnetCoreRestore);
    public Target Build => definition => definition.Inherit<ICanBuildWithDotNetCore>(dotNetCore => dotNetCore.DotnetCoreBuild);
    public Target Test => definition => definition.Inherit<ICanTestWithDotNetCore>(dotNetCore => dotNetCore.DotnetCoreTest);
    public Target Pack => definition => definition.Inherit<ICanPackWithDotNetCore>(dotNetCore => dotNetCore.DotnetCorePack)
       .DependsOn(Clean)
       .After(Test);

    [Solution(GenerateProjects = true)]
    Solution Solution { get; } = null!;

    Nuke.Common.ProjectModel.Solution IHaveSolution.Solution => Solution;

    [OptionalGitRepository]
    public GitRepository? GitRepository { get; }

    [ComputedGitVersion]
    public GitVersion GitVersion { get; } = null!;

    [Parameter("Configuration to build")]
    public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    public Target Lint => definition => definition;
}