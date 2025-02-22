namespace Rocket.Surgery.Airframe.Defaults
{
    [ReferenceDefault]
    public partial class Thing
    {
    }

    public partial class Thing
    {
        public static Thing Default { get; } = new Thing();
    }
}