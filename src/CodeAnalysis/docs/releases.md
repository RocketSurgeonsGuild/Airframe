# Published CodeAnalysis packages

The release records use published `Rocket.Surgery.Airframe.CodeAnalysis` package contents.
Repository tags and the project's local `Version` property do not establish that an analyzer
was distributed to package consumers.

## Package audit

Verified on 2026-10-29 against the [NuGet version index](https://api.nuget.org/v3-flatcontainer/rocket.surgery.airframe.codeanalysis/index.json)
and the `.nupkg` files downloaded from that feed. The index lists 0.12.0, 0.12.1, and 0.12.2.
The project's `1.0.0.9` version is not published on NuGet.org and is not a shipped baseline.

| CodeAnalysis version | Package contents | Discoverable rules | New rules |
|----------------------|------------------|--------------------|-----------|
| [0.12.0](https://www.nuget.org/packages/Rocket.Surgery.Airframe.CodeAnalysis/0.12.0) | No analyzer or code fix assemblies | 0 | 0 |
| [0.12.1](https://www.nuget.org/packages/Rocket.Surgery.Airframe.CodeAnalysis/0.12.1) | Analyzer and code fix assemblies under `analyzers/dotnet/cs/` | 22 | 22 |
| [0.12.2](https://www.nuget.org/packages/Rocket.Surgery.Airframe.CodeAnalysis/0.12.2) | Analyzer and code fix assemblies under `analyzers/dotnet/cs/` | 37 | 15 |

The 0.12.0 manifest declares dependencies on `Rocket.Surgery.Airframe.Analyzers` and
`Rocket.Surgery.Airframe.CodeFixes` 0.12.0 with `exclude="Build,Analyzers"`. No rules are
attributed to that CodeAnalysis release. The 0.12.1 and 0.12.2 packages bundle their assemblies
directly and have no package dependencies.

For each bundled analyzer assembly, the audit enumerated concrete `DiagnosticAnalyzer` types
with a directly applied `DiagnosticAnalyzerAttribute`, instantiated them, and read their
`SupportedDiagnostics`. Rule IDs were deduplicated, then category, default severity, and
enabled-by-default status were compared across packages and with the current assembly.
All 37 current descriptors match published 0.12.2 on those properties.

The [shipped file](../../Analyzers/AnalyzerReleases.Shipped.md) therefore starts at 0.12.1.
The [unshipped file](../../Analyzers/AnalyzerReleases.Unshipped.md) has no rule additions,
removals, or metadata changes. Behavior and code fix changes since the published package are
listed separately in the [package README](../README.md#release-status).

## Package provenance

These source commits come from each downloaded package's `.nuspec`, rather than inferred tags.
The SHA-256 values identify the exact `.nupkg` files inspected.

| Version | Repository commit | Package SHA-256 |
|---------|-------------------|-----------------|
| 0.12.0 | `5458bf844d9f2eb15f032325c76ad2a02c476a9b` | `cbb01bfb6d1c51e4e0d0cd445f53f5c491ed5fa36ba7a82fd8ca0d41bf6846b3` |
| 0.12.1 | `a3f83fa57637c9de345a18c73d7cfc1df6b05ef9` | `38285e28b00171364c2d3a325c2cf5b24030a127d3d5daa5d5c47ef5491ad037` |
| 0.12.2 | `131f60f4db1954157c77b2e715b8d91adf3b9f04` | `2b74fca91973b06ecdbc4eec832cab7bd4f9f4170679f1694ba7bd7aecfb1ed0` |

When publishing another version, inspect the resulting package before moving entries out of
the unshipped file. Update the [rule index](README.md) and
[README release status](../README.md#release-status) from the same package evidence.
