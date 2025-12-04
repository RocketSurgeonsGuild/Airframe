namespace Rocket.Surgery.Airframe.Defaults.Property;

internal static class DefaultsTemplate
{
    public const string Template =
        """
        public partial class {{class}}
        {
            public static {{class}} {{property}} { get; } => new({{arguments}});
        }
        """;
}