using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering an <see cref="IDependencyModule{IServiceCollection}"/>.
/// </summary>
public static class ServiceCollectionModuleExtensions
{
    /// <summary>
    /// Adds the provided <see cref="IDependencyModule{IServiceCollection}"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="module">The module.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The service collection with dependencies registered.</returns>
    public static IServiceCollection AddModule<T>(this IServiceCollection serviceCollection, T module)
        where T : IDependencyModule<IServiceCollection> => module.Register(serviceCollection);

    /// <summary>
    /// Adds the provided <see cref="IDependencyModule{IServiceCollection}"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The service collection with dependencies registered.</returns>
    public static IServiceCollection AddModule<T>(this IServiceCollection serviceCollection)
        where T : IDependencyModule<IServiceCollection>, new() => new T().Register(serviceCollection);
}