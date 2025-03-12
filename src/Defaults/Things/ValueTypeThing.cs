using System.Diagnostics.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults.Things;

[ReferenceDefault]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Sandbox")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "Sandbox")]
public partial class ValueTypeThing
{
    public ValueTypeThing(int value) => Value = value;

    public int Value { get; }
}

public partial class ValueTypeThing
{
    public static ValueTypeThing Default { get; } = new ValueTypeThing(default);
}