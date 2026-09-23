# Rocket.Surgery.Airframe.CodeAnalysis

A set of analyzers and code fixes for common patterns found in Airframe based applications.

## Analyzers

- MVVM patterns
- ReactiveUI best practices
- Airframe specific coding standards

## Usage

This package is a development dependency and does not add any runtime dependencies to your project.

## Upgrading from 0.12.0 or earlier: rules that were silent now report

Every rule from RSA1001 to RSA3002 was previously inert. `DiagnosticAnalyzerAttribute` sat on the
abstract base classes, and Roslyn does not inherit that attribute when it discovers analyzers, so
the compiler loaded none of the concrete rules. RSA1007 was the only rule unaffected.

That is fixed, which means **upgrading turns on rules that have never reported before**. Across the
Airframe source tree the fix surfaces 109 RSA3002 and 6 RSA1007 diagnostics that were always true
and never reportable. Expect a comparable jump in your own build, and expect it to break the build
outright if you run with `TreatWarningsAsErrors`.

To stage the adoption, set the rules you are not ready for to `none` or `suggestion` in
`.editorconfig` and raise them as you clear them:

```ini
[*.cs]
dotnet_diagnostic.RSA3002.severity = suggestion
```
