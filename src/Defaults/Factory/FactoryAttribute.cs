namespace Rocket.Surgery.Airframe.Defaults.Factory;

internal static class FactoryAttribute
{
    // lang=csharp
    public static string Source { get; } = $$"""
        using System;
        using System.Diagnostics;

        namespace Rocket.Surgery.Airframe.Defaults
        {
            {{Defaults.CodeGenerationAttribute}}
            [AttributeUsage(AttributeTargets.Constructor)]
            [Conditional("CODEGEN")]
        
            internal class DefaultsFactoryAttribute : Attribute
            {
                public DefaultsFactoryAttribute()
                    : this("Create")
                {
                }
            
                public DefaultsFactoryAttribute(string factoryName) => FactoryName = factoryName;
            
                public string FactoryName { get; set; }
            }
        }
        """;

    public const string AttributeName = "Rocket.Surgery.Airframe.Defaults.DefaultsFactoryAttribute";
}