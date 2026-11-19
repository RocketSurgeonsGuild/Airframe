; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
RSA2016 | Design | Warning | RSA2016_RedundantTypeAlias
RSA3006 | Performance | Warning | RSA3006_ObservableFromEvent

; RSA3002 now covers anonymous methods and checks captures in the function being changed.
