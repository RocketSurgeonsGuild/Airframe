using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Rocket.Surgery.Nuke;
using System.Collections.Generic;
using System.Linq;
using Rocket.Surgery.Nuke.MsBuild;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class RocketSurgeryReactiveUI : MsBuild, IMsBuild
{
    /// <summary>
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    /// </summary>

    public static int Main() => Execute<RocketSurgeryReactiveUI>(x => x.Default);

    public new Target Restore => _ => _.With(this, MsBuild.Restore);

    public new Target Build => _ => _.With(this, MsBuild.Build);

    public new Target Test => _ => _.With(this, MsBuild.Test);

    public new Target Pack => _ => _.With(this, MsBuild.Pack);

    Target Default => _ => _
        .DependsOn(Restore)
        .DependsOn(Build)
        .DependsOn(Test)
        .DependsOn(Pack);
}