using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Rocket.Surgery.Nuke;
using System.Collections.Generic;
using System.Linq;
using Rocket.Surgery.Nuke.MsBuild;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : MsBuild
{
    /// <summary>
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    /// </summary>

    public static int Main() => Execute<Build>(x => x.Default);

    Target Default => _ => _.DependsOn(NetFramework);
}