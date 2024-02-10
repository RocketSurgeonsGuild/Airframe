using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a startup options configurator.
/// </summary>
public class StartupOption
{
    private readonly IServiceCollection _serviceCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupOption"/> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    public StartupOption(IServiceCollection serviceCollection) =>
        _serviceCollection = serviceCollection;

    /// <summary>
    /// Adds an <see cref="IStartupOperation"/> to the container.
    /// </summary>
    /// <typeparam name="T">The operation type.</typeparam>
    /// <returns>The option configuration.</returns>
    public StartupOption AddOperation<T>()
        where T : class, IStartupOperation
    {
        _serviceCollection.AddTransient<IStartupOperation, T>();

        return this;
    }
}