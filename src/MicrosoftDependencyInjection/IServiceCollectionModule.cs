using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Interface that represents a module that groups registrations to the <see cref="IServiceCollection"/>.
    /// </summary>
    public interface IServiceCollectionModule
    {
        /// <summary>
        /// Loads the provided <see cref="IServiceCollection"/> with <see cref="ServiceDescriptor"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection with dependencies registered.</returns>
        IServiceCollection Load(IServiceCollection serviceCollection);
    }
}