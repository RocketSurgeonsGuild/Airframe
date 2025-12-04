namespace Rocket.Surgery.Airframe.Defaults.Factory;

public class FactoryTemplate
{
    // lang=csharp
    public const string Template =
        """
        public partial class {{class}}
        {
            public static {{class}} {{method}}({{parameters}}) => new({{parameters}});
        }
        """;
}