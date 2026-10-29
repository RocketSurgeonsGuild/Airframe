using System.Reflection;
using Rocket.Surgery.Airframe.Defaults.Property;

namespace Rocket.Surgery.Airframe.Defaults;

internal static class Defaults
{
    public static string Version { get; } =
        typeof(DefaultsGenerator).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
     ?? typeof(DefaultsGenerator).Assembly.GetName().Version.ToString();

    public static string CodeGenerationAttribute { get; } = $@"[System.CodeDom.Compiler.GeneratedCode(""Rocket.Surgery.Airframe.Defaults"", ""{Version}"")]";
}