using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Rocket.Surgery.Nuke;
using Rocket.Surgery.Nuke.DotNetCore;
using Rocket.Surgery.Nuke.MsBuild;
using System;
using System.Linq;

[PublicAPI]
[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[PackageIcon("https://raw.githubusercontent.com/RocketSurgeonsGuild/graphics/master/png/social-square-thrust-rounded.png")]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
// [EnsureGitHooks(GitHook.CommitMsg)]
public partial class AirframeBuild : NukeBuild,
                        ICanClean,
                        ICanRestoreWithMsBuild,
                        ICanTestWithDotNetCoreNoBuild,
                        ICanPackWithMsBuild,
                        ICanUpdateReadme,
                        IHaveDataCollector,
                        IHaveConfiguration<Configuration>,
                        IGenerateCodeCoverageReport,
                        IGenerateCodeCoverageSummary,
                        IGenerateCodeCoverageBadges,
                        ICanLint
{
    /// <summary>
    /// Support plugins are available for:
    /// - JetBrains ReSharper        https://nuke.build/resharper
    /// - JetBrains Rider            https://nuke.build/rider
    /// - Microsoft VisualStudio     https://nuke.build/visualstudio
    /// - Microsoft VSCode           https://nuke.build/vscode
    /// </summary>
    public static int Main() => Execute<AirframeBuild>(x => x.Default);

    [OptionalGitRepository]
    public GitRepository? GitRepository { get; }

    [ComputedGitVersion]
    public GitVersion GitVersion { get; } = null!;

    private Target Default => _ => _
       .DependsOn(Restore)
       .DependsOn(Build)
       .DependsOn(Test)
       .DependsOn(Pack);

    public Target Build => _ => _.Inherit<ICanBuildWithMsBuild>(x => x.NetBuild);

    public Target Pack => _ => _
       .Inherit<ICanPackWithMsBuild>(x => x.NetPack)
       .DependsOn(Clean);
    public Target Clean => _ => _.Inherit<ICanClean>(x => x.Clean);
    public Target Restore => _ => _.Inherit<ICanRestoreWithMsBuild>(x => x.NetRestore);
    public Target Test => _ => _.Inherit<ICanTestWithDotNetCoreNoBuild>(x => x.CoreTest);
    public Target BuildVersion => _ => _
       .Inherit<IHaveBuildVersion>(x => x.BuildVersion)
       .Before(Default)
       .Before(Clean);

    [Parameter("Configuration to build")]
    public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;
}