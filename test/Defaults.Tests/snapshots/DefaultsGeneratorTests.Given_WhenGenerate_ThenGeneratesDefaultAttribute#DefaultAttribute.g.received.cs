//HintName: Rocket.Surgery.Airframe.Defaults/Rocket.Surgery.Airframe.Defaults.DefaultsGenerator/DefaultAttribute.g.cs
#nullable enable
using System;
using System.Diagnostics;

namespace Rocket.Surgery.Airframe.Defaults
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Airframe.Defaults", "1.0.0+b4aaf0cae66f2b168289a092ab7b7d18a59d97e6")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Conditional("CODEGEN")]
    internal class DefaultAttribute : Attribute
    {
        public DefaultAttribute() => PropertyName = "Default";

        public string PropertyName { get; set; }
    }
}