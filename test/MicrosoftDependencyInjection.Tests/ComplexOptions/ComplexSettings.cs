using Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.FlatOptions;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.ComplexOptions;

public class ComplexSettings
{
    public FlatSettings Flat { get; set; }

    public LevelSettings Levels { get; set; }
}