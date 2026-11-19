# Airframe
A collection of abstract classes that extend ReactiveUI implementations

## Requirements

Build with the .NET 11 SDK selected by `global.json` (11.0.100 or a later patch in
the 11.0.1xx feature band). Runtime packages target .NET 11; the analyzer and
compatibility projects retain their `netstandard2.0` targets.

The main solution requires no workloads. To build and pack the Apple package,
install the .NET 11 iOS workload with `dotnet workload install ios`, then build
`Apple.sln`. CI keeps this workload in its separate Apple job.

Benchmarks use BenchmarkDotNet 0.16.0-preview.2 for .NET 11 runtime support.

## Code cleanup

After restoring the repository, run `./build.sh Cleanup` to apply the selected code fixes, or
`./build.sh VerifyCleanup` to check the same selection without writing source files. The verify
target returns a nonzero exit code when cleanup is needed and is the entry point for CI.

Both targets use `info` severity so the selected suggestions participate. Their shared selection
is in `.build/CleanupCommands.cs`: simplify names and member access (IDE0001/IDE0002), remove
unused imports (IDE0005), prefer `var` (IDE0007), use target-typed construction (IDE0090), and
remove redundant type aliases (RSA2016). They run the native style and Airframe analyzer passes
without a general whitespace-formatting pass or unrelated analyzer fixes. The existing NUKE
`DotNetFormat` lint integration retains its behavior.
The alias pass runs before unused-import cleanup so the namespace import that makes an alias
redundant is still available when RSA2016 checks it.

Scope a cleanup while adopting the profile:

```sh
./build.sh Cleanup --cleanup-project src/Core/Core.csproj --cleanup-files src/Core/Example.cs
./build.sh VerifyCleanup --cleanup-project src/Core/Core.csproj --cleanup-files src/Core/Example.cs
```

Omit `--cleanup-files` to check every source file in the selected project/solution. Paths are
relative to the repository root; separate multiple files with semicolons and quote the list,
for example `--cleanup-files "src/Core/First.cs;src/Core/Second.cs"`. Quoting also preserves spaces
inside a file path. Use the same scope for both
commands. After a separate restore, `--skip Restore` skips that prerequisite. Cleanup builds
the current Airframe analyzer and code-fix assemblies and loads them only for these formatter
processes, so RSA2016 does not depend on a previously published package. The NUKE host requires
the .NET 11 runtime, and the SDK must support the selected project's target framework. Use the
same SDK version locally and in CI. Existing cleanup debt is left in place until
an explicit scoped or repository-wide `Cleanup` is requested; CI can adopt `VerifyCleanup` for
that same scope once it is clean.

# Status
<!-- badges -->
[![github-release-badge]][github-release]
[![github-license-badge]][github-license]
[![codecov-badge]][codecov]
<!-- badges -->

<!-- history badges -->
| GitHub Actions                    |
|-----------------------------------|
| [![github-badge]][github]         |
| [![github-history-badge]][github] |
<!-- history badges -->

### NuGet

<!-- nuget packages -->
| Package                            | NuGet                                                                                    |
|------------------------------------|------------------------------------------------------------------------------------------|
| Rocket.Surgery.Airframe            | [![nuget-version-badge]![nuget-downloads-badge]][nuget]                                  |
| Rocket.Surgery.Airframe.Forms      | [![forms-nuget-version-badge]![forms-nuget-downloads-badge]][forms-nuget]                |
| Rocket.Surgery.Airframe.ViewModels | [![viewmodels-nuget-version-badge]![viewmodels-nuget-downloads-badge]][viewmodels-nuget] |
<!-- nuget packages -->

<!-- generated references -->
[github-release]: https://github.com/RocketSurgeonsGuild/Airframe/releases/latest
[github-release-badge]: https://img.shields.io/github/release/RocketSurgeonsGuild/Airframe.svg?logo=github&style=flat "Latest Release"
[github-license]: https://github.com/RocketSurgeonsGuild/Airframe/blob/master/LICENSE
[github-license-badge]: https://img.shields.io/github/license/RocketSurgeonsGuild/Airframe.svg?style=flat "License"
[codecov]: https://codecov.io/gh/RocketSurgeonsGuild/Airframe
[codecov-badge]: https://img.shields.io/codecov/c/github/RocketSurgeonsGuild/Airframe.svg?color=E03997&label=codecov&logo=codecov&logoColor=E03997&style=flat "Code Coverage"

[azurepipelines]: https://dev.azure.com/RocketSurgeonsGuild/Libraries/_apis/build/status/RSG.Airframe?branchName=master
[azurepipelines-badge]: https://img.shields.io/azure-devops/build/RocketSurgeonsGuild/Libraries/27.svg?color=98C6FF&label=azure%20pipelines&logo=azuredevops&logoColor=98C6FF&style=flat "Azure Pipelines Status"
[azurepipelines-history]: https://dev.azure.com/RocketSurgeonsGuild/Libraries/_build/latest?definitionId=27&branchName=master
[azurepipelines-history-badge]: https://buildstats.info/azurepipelines/chart/RocketSurgeonsGuild/Libraries/27?includeBuildsFromPullRequest=false "Azure Pipelines History"

[github]: https://github.com/RocketSurgeonsGuild/Airframe/actions?query=workflow%3Aci
[github-badge]: https://img.shields.io/github/workflow/status/RocketSurgeonsGuild/Airframe/ci.svg?label=github&logo=github&color=b845fc&logoColor=b845fc&style=flat "GitHub Actions Status"
[github-history-badge]: https://buildstats.info/github/chart/RocketSurgeonsGuild/Airframe?includeBuildsFromPullRequest=false "GitHub Actions History"
[nuget]: https://www.nuget.org/packages/Rocket.Surgery.Airframe/
[nuget-version-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.Airframe.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-badge]: https://img.shields.io/nuget/dt/Rocket.Surgery.Airframe.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"

[forms-nuget]: https://www.nuget.org/packages/Rocket.Surgery.Airframe.Forms/
[forms-nuget-version-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.Airframe.Forms.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[forms-nuget-downloads-badge]: https://img.shields.io/nuget/dt/Rocket.Surgery.Airframe.Forms.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"

[viewmodels-nuget]: https://www.nuget.org/packages/Rocket.Surgery.Airframe.ViewModels/
[viewmodels-nuget-version-badge]: https://img.shields.io/nuget/v/Rocket.Surgery.Airframe.ViewModels.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[viewmodels-nuget-downloads-badge]: https://img.shields.io/nuget/dt/Rocket.Surgery.Airframe.ViewModels.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
<!-- generated references -->
