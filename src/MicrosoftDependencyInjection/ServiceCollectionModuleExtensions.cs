using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for registering an <see cref="IServiceCollectionModule"/>.
    /// </summary>
    public static class ServiceCollectionModuleExtensions
    {
        /// <summary>
        /// Adds the provided <see cref="IServiceCollectionModule"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="module">The module.</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The service collection with dependencies registered.</returns>
        public static IServiceCollection AddModule<T>(this IServiceCollection serviceCollection, T module)
            where T : IServiceCollectionModule => module.Load(serviceCollection);

        /// <summary>
        /// Adds the provided <see cref="IServiceCollectionModule"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The service collection with dependencies registered.</returns>
        public static IServiceCollection AddModule<T>(this IServiceCollection serviceCollection)
            where T : IServiceCollectionModule, new() => new T().Load(serviceCollection);
    }
}