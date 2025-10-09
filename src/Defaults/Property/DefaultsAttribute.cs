namespace Rocket.Surgery.Airframe.Defaults.Property;

internal static class DefaultsAttribute
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
                                                 internal class DefaultsAttribute : Attribute
                                                 {
                                                     public DefaultsAttribute()
                                                         : this("Default")
                                                     {
                                                     }
                                             
                                                     public DefaultsAttribute(string propertyName) => PropertyName = propertyName;
                                             
                                                     public string PropertyName { get; set; }
                                                 }
                                             }
                                             """;

    public const string AttributeName = "Rocket.Surgery.Airframe.Defaults.DefaultsAttribute";
}