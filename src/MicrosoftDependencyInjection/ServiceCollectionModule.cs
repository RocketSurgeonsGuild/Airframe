using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a module to register dependencies against an <see cref="IServiceCollection"/>.
/// </summary>
public abstract class ServiceCollectionModule : IDependencyModule<IServiceCollection>
{
    /// <inheritdoc/>
    IServiceCollection IDependencyModule<IServiceCollection>.Register(IServiceCollection container) => Add(container);

    /// <summary>
    /// Loads the provided <see cref="IServiceCollection"/> with <see cref="ServiceDescriptor"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection with dependencies registered.</returns>
    protected abstract IServiceCollection Add(IServiceCollection serviceCollection);
}