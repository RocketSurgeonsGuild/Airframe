using DryIoc;

namespace Rocket.Surgery.Airframe.Composition;

/// <summary>
/// Represents a module to load for dependency inversion.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Loads module registrations.
    /// </summary>
    /// <param name="registrar">The dependency registrar.</param>
    // Here we are using registration role of DryIoc Container for the builder
    void Load(IRegistrator registrar);
}