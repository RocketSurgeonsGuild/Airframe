//HintName: Rocket.Surgery.Airframe.Defaults/Rocket.Surgery.Airframe.Defaults.DefaultsGenerator/DefaultAttribute.g.cs
#nullable enable
using System;
using System.Diagnostics;

namespace Rocket.Surgery.Airframe.Defaults
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Conditional("CODEGEN")]
    internal class DefaultAttribute : Attribute
    {
        public DefaultAttribute() => PropertyName = "Default";

        public string PropertyName { get; set; }
    }
}