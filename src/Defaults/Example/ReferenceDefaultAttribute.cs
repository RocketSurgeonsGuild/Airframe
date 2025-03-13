using System;

namespace Rocket.Surgery.Airframe.Defaults.Example;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class ReferenceDefaultAttribute : Attribute
{
    public ReferenceDefaultAttribute()
        : this("Default")
    {
    }

    public ReferenceDefaultAttribute(string propertyName) => PropertyName = propertyName;

    public string PropertyName { get; set; }
}
