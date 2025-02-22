using System;

namespace Rocket.Surgery.Airframe.Defaults;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class ReferenceDefaultAttribute : Attribute
{
    public ReferenceDefaultAttribute() => PropertyName = "Default";

    public string PropertyName { get; set; }
}