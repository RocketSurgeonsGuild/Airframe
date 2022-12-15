using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a configuration options configurator.
/// </summary>
public class ConfigurationOption
{
    private readonly IServiceCollection _serviceCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationOption"/> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    public ConfigurationOption(IServiceCollection serviceCollection) =>
        _serviceCollection = serviceCollection;

    /// <summary>
    /// Adds an <see cref="IStartupOperation"/> to the container.
    /// </summary>
    /// <typeparam name="T">The operation type.</typeparam>
    /// <returns>The option configuration.</returns>
    public ConfigurationOption ConfigureOption<T>()
        where T : class => this;
}