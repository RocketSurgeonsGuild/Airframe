using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Represents a configuration options configurator.
    /// </summary>
    public class ConfigurationOptions
    {
        private readonly IServiceCollection _serviceCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationOptions"/> class.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        public ConfigurationOptions(IServiceCollection serviceCollection) => _serviceCollection = serviceCollection;

        /// <summary>
        /// Adds an <see cref="IStartupOperation"/> to the container.
        /// </summary>
        /// <typeparam name="T">The operation type.</typeparam>
        /// <returns>The option configuration.</returns>
        public ConfigurationOptions ConfigureOption<T>()
            where T : class
        {
            _serviceCollection
               .AddOptions<T>()
               .Configure((T settings, IConfiguration config) => config.Bind(settings));

            return this;
        }

        /// <summary>
        /// Configure the section as <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The section type.</typeparam>
        /// <returns>The configuration options.</returns>
        public ConfigurationOptions ConfigureSection<T>()
            where T : class => ConfigureSection<T>(typeof(T).Name);

        /// <summary>
        /// Configure the section as <see cref="T"/>.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <typeparam name="T">The section type.</typeparam>
        /// <returns>The configuration options.</returns>
        public ConfigurationOptions ConfigureSection<T>(string section)
            where T : class
        {
            _serviceCollection
               .AddOptions<T>()
               .Configure((T settings, IConfiguration config) => config.GetSection(section).Bind(settings));

            return this;
        }
    }
}