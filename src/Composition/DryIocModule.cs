using DryIoc;

namespace Rocket.Surgery.Airframe.Composition
{
    /// <summary>
    /// Base abstraction for an <see cref="IModule"/>.
    /// </summary>
    public abstract class DryIocModule : IModule
    {
        /// <summary>
        /// Loads registrations to the <see cref="IRegistrator"/>.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        public abstract void Load(IRegistrator registrar);
    }
}