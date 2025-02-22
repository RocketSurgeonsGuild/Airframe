//HintName: Rocket.Surgery.Airframe.Defaults/Rocket.Surgery.Airframe.Defaults.DefaultsGenerator/DefaultAttribute.g.cs
#nullable enable
using System;
using System.Diagnostics;

namespace Rocket.Surgery.Airframe.Defaults
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Airframe.Defaults", "1.0.0+aa42bbf314dfcb883919c81eb6ea8a2af433e63f")]
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