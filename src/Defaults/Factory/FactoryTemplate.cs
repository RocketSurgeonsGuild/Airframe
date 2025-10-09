namespace Rocket.Surgery.Airframe.Defaults.Factory;

public class FactoryTemplate
{
    // lang=csharp
    private const string Template =
        """
        public partial class Thing
        {
            public static Thing Create() => new Thing();
        }
        """;
}

public partial class Thing
{
    public static Thing Create() => new Thing();
}