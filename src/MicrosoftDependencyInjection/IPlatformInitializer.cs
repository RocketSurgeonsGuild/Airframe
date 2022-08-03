using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Represents an initializer for a specific platform.
    /// </summary>
    public interface IPlatformInitializer
    {
        /// <summary>
        /// Initialize the platform.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection with platform dependencies registered.</returns>
        public IServiceCollection Initialize(IServiceCollection serviceCollection);
    }
}