using DryIoc;

namespace Rocket.Surgery.Airframe.Composition;

/// <summary>
/// Base platform registration object.
/// </summary>
public abstract class PlatformRegistrarBase : IPlatformRegistrar
{
    /// <inheritdoc />
    public abstract void RegisterPlatformServices(IRegistrator registrator);
}