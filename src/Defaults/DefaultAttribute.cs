namespace Rocket.Surgery.Airframe.Defaults;

internal class DefaultAttribute
{
    public static string Source() => $$"""
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
                                               public DefaultAttribute() => PropertyName = "Default";
                                       
                                               public string PropertyName { get; set; }
                                           }
                                       }
                                       """;
}