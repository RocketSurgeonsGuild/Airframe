namespace Rocket.Surgery.Airframe.Defaults;

internal static class DefaultAttribute
{
    // lang=csharp
    public static string Source { get; } = $$"""
                                             #nullable enable
                                             using System;
                                             using System.Diagnostics;

                                             namespace Rocket.Surgery.Airframe.Defaults
                                             {
                                                 {{Defaults.CodeGenerationAttribute}}
                                                 [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
                                                 [Conditional("CODEGEN")]
                                                 internal class DefaultAttribute : Attribute
                                                 {
                                                     public DefaultAttribute()
                                                         : this("Default")
                                                     {
                                                     }
                                             
                                                     public DefaultAttribute(string propertyName) => PropertyName = propertyName;
                                             
                                                     public string PropertyName { get; set; }
                                                 }
                                             }
                                             """;
}