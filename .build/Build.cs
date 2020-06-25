using Nuke.Common;
using Nuke.Common.Execution;
using Rocket.Surgery.Nuke;
using Rocket.Surgery.Nuke.MsBuild;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Airframe : MsBuild, IMsBuild
{
    /// <summary>
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    /// </summary>

    public static int Main() => Execute<Airframe>(x => x.Default);

    public Target Restore => _ => _.With(this, MsBuild.Restore);

    public Target Build => _ => _.With(this, MsBuild.Build);

    public Target Test => _ => _.With(this, MsBuild.Test);

    public Target Pack => _ => _.With(this, MsBuild.Pack);

    Target Default => _ => _
        .DependsOn(Restore)
        .DependsOn(Build)
        .DependsOn(Test)
        .DependsOn(Pack);
}