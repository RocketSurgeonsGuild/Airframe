using DryIoc;

namespace Rocket.Surgery.Airframe.Composition
{
    /// <summary>
    /// Represents a platform specific registration handler.
    /// </summary>
    public interface IPlatformRegistrar
    {
        /// <summary>
        /// Register platform specific services.
        /// </summary>
        /// <param name="registrator">The registrator.</param>
        void RegisterPlatformServices(IRegistrator registrator);
    }
}