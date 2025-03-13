using System.Diagnostics.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults.Example;

[ReferenceDefault]
public partial class ReferenceTypeThing
{
    public ReferenceTypeThing(NestedReferenceTypeThing thingOne) => ThingOne = thingOne;

    public NestedReferenceTypeThing ThingOne { get; }
}

[ReferenceDefault] // Analysis, Generate
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Sandbox")]
public partial class NestedReferenceTypeThing
{
}