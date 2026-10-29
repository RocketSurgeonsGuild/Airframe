//HintName: Rocket.Surgery.Airframe.Defaults/Rocket.Surgery.Airframe.Defaults.Property.DefaultsGenerator/DefaultsAttribute.g.cs
#nullable enable
using System;
using System.Diagnostics;

namespace Rocket.Surgery.Airframe.Defaults
{
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