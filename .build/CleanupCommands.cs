using System.Collections.Generic;

/// <summary>The diagnostic selection shared by explicit cleanup and its CI verification.</summary>
internal static class CleanupCommands
{
    internal static IEnumerable<string[]> Create(string project, IReadOnlyCollection<string> files, bool verify)
    {
        string[] scope = files.Count == 0 ? [] : ["--include", .. files];
        string[] verification = verify ? ["--verify-no-changes"] : [];

        // Remove redundant aliases before IDE0005 can discard the namespace imports that make
        // those aliases redundant. The reverse order would preserve an otherwise needless alias.
        yield return
        [
            "format", "analyzers", project, "--no-restore", "--severity", "info", "--diagnostics", "RSA2016",
            .. scope, .. verification
        ];

        yield return
        [
            "format", "style", project, "--no-restore", "--severity", "info", "--diagnostics",
            "IDE0001", "IDE0002", "IDE0005", "IDE0007", "IDE0090",
            .. scope, .. verification
        ];
    }
}
