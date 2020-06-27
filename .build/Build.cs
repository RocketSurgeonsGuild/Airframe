using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nuke.Airframe;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Rocket.Surgery.Nuke;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.DotNetCore;
using Rocket.Surgery.Nuke.GithubActions;
using Rocket.Surgery.Nuke.MsBuild;
using Rocket.Surgery.Nuke.Xamarin;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;

[PublicAPI]
[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[PackageIcon("https://raw.githubusercontent.com/RocketSurgeonsGuild/graphics/master/png/social-square-thrust-rounded.png")]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
public partial class Solution : NukeBuild,
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
    public static int Main() => Execute<Solution>(x => x.Default);

    [OptionalGitRepository]
    public GitRepository? GitRepository { get; }

    private Target Default => _ => _
       .DependsOn(Restore)
       .DependsOn(Build)
       .DependsOn(Test)
       .DependsOn(Pack);

    public Target Build => _ => _.Inherit<ICanBuildWithMsBuild>(x => x.NetBuild);

    public Target Pack => _ => _.Inherit<ICanPackWithMsBuild>(x => x.NetPack)
       .DependsOn(Clean);

    [ComputedGitVersion]
    public GitVersion GitVersion { get; } = null!;

    public Target Clean => _ => _.Inherit<ICanClean>(x => x.Clean);
    public Target Restore => _ => _.Inherit<ICanRestoreWithMsBuild>(x => x.NetRestore);
    public Target Test => _ => _.Inherit<ICanTestWithDotNetCoreNoBuild>(x => x.CoreTest);

    public Target BuildVersion => _ => _.Inherit<IHaveBuildVersion>(x => x.BuildVersion)
       .Before(Default)
       .Before(Clean);

    [Parameter("Configuration to build")]
    public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;
}
