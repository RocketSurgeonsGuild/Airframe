using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="Microsoft.Extensions.DependencyInjection"/>.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IPlatformInitializer"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="platformInitializer">The platform initializer.</param>
        /// <returns>The service collection with platform dependencies registered.</returns>
        public static IServiceCollection AddPlatform(this IServiceCollection serviceCollection, IPlatformInitializer platformInitializer) =>
            platformInitializer.Initialize(serviceCollection);

        /// <summary>
        /// Adds the <see cref="IPlatformInitializer"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="target">The target that owns the platform initializer.</param>
        /// <param name="startup">The startup.</param>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <returns>The service collection with platform dependencies registered.</returns>
        public static IServiceCollection AddPlatform<TTarget>(this IServiceCollection serviceCollection, TTarget target, Func<TTarget, IPlatformInitializer> startup) =>
            startup(target).Initialize(serviceCollection);

        /// <summary>
        /// Registers an <see cref="IApplicationStartup"/> to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="options">The startup options.</param>
        /// <typeparam name="T">The startup type.</typeparam>
        /// <returns>The service collection with startup dependencies registered.</returns>
        public static IServiceCollection AddStartup<T>(this IServiceCollection serviceCollection, Action<StartupOption>? options = null)
            where T : class, IApplicationStartup
        {
            serviceCollection.AddTransient<IApplicationStartup, T>();

            if (options != null)
            {
                var startupOption = new StartupOption(serviceCollection);
                options.Invoke(startupOption);
            }

            return serviceCollection;
        }

        /// <summary>
        /// Configures the app settings for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection with configuration dependencies registered.</returns>
        public static IServiceCollection ConfigureAppSettings(this IServiceCollection serviceCollection) =>
            ConfigureAppSettings(
                serviceCollection,
                new ConfigurationBuilder());

        /// <summary>
        /// Configures the app settings for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="builder">The configuration builder.</param>
        /// <returns>The service collection with configuration dependencies registered.</returns>
        public static IServiceCollection ConfigureAppSettings(this IServiceCollection serviceCollection, Action<IConfigurationBuilder> builder)
        {
            var configurationBuilder = new ConfigurationBuilder();
            builder.Invoke(configurationBuilder);

            return ConfigureAppSettings(serviceCollection, configurationBuilder);
        }

        /// <summary>
        /// Configures the app settings for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configurationBuilder">The configuration builder.</param>
        /// <returns>The service collection with configuration dependencies registered.</returns>
        public static IServiceCollection ConfigureAppSettings(this IServiceCollection serviceCollection, IConfigurationBuilder configurationBuilder) =>
            ConfigureAppSettings(serviceCollection, configurationBuilder.Build());

        /// <summary>
        /// Configures the app settings for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection with configuration dependencies registered.</returns>
        public static IServiceCollection ConfigureAppSettings(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder();

            return serviceCollection.AddSingleton<IConfiguration>(provider =>
                {
                    provider.GetService<Action<IConfigurationBuilder>>()?.Invoke(builder);

                    return builder.AddConfiguration(configuration).Build();
                });
        }

        /// <summary>
        /// Configures the <see cref="IOptions{TOptions}"/> for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="T">The option type.</typeparam>
        /// <returns>The service collection with options registered.</returns>
        public static IServiceCollection ConfigureSectionAsOptions<T>(this IServiceCollection serviceCollection)
            where T : class => ConfigureSectionAsOptions<T>(serviceCollection, typeof(T).Name);

        /// <summary>
        /// Configures the <see cref="IOptions{TOptions}"/> for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="section">The app settings section.</param>
        /// <typeparam name="T">The option type.</typeparam>
        /// <returns>The service collection with options registered.</returns>
        public static IServiceCollection ConfigureSectionAsOptions<T>(this IServiceCollection serviceCollection, string section)
            where T : class
        {
            serviceCollection
               .AddOptions<T>()
               .Configure((T settings, IConfiguration config) => config.GetSection(section).Bind(settings));

            return serviceCollection;
        }

        /// <summary>
        /// Configures the <see cref="IOptions{TOptions}"/> for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="T">The option type.</typeparam>
        /// <returns>The service collection with options registered.</returns>
        public static IServiceCollection ConfigureOptions<T>(this IServiceCollection serviceCollection)
            where T : class
        {
            serviceCollection
               .AddOptions<T>()
               .Configure((T settings, IConfiguration config) => config.Bind(settings));

            return serviceCollection;
        }

        /// <summary>
        /// Registers a builder delegate for use in application configuration construction.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configurationBuilder">The configuration builder.</param>
        /// <returns>The service collection with startup dependencies registered.</returns>
        public static IServiceCollection ConfigureBuilder(this IServiceCollection serviceCollection, Action<IConfigurationBuilder> configurationBuilder) =>
            serviceCollection.AddSingleton<Action<IConfigurationBuilder>>(_ => configurationBuilder);

        /// <summary>
        /// Registers an <see cref="IApplicationStartup"/> to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configuration">The configuration builder.</param>
        /// <param name="options">The startup options.</param>
        /// <returns>The service collection with startup dependencies registered.</returns>
        public static IServiceCollection ConfigureSettings(
            this IServiceCollection serviceCollection,
            Action<IConfigurationBuilder>? configuration,
            Action<ConfigurationOptions>? options)
        {
            if (options == null)
            {
                return serviceCollection;
            }

            var configurationBuilder = new ConfigurationBuilder();

            configuration?.Invoke(configurationBuilder);

            ConfigureAppSettings(serviceCollection, configurationBuilder);

            var startupOption = new ConfigurationOptions(serviceCollection);
            options.Invoke(startupOption);

            return serviceCollection;
        }

        /// <summary>
        /// Registers an <see cref="IApplicationStartup"/> to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="options">The startup options.</param>
        /// <returns>The service collection with startup dependencies registered.</returns>
        public static IServiceCollection ConfigureOptions(
            this IServiceCollection serviceCollection,
            Action<ConfigurationOptions>? options)
        {
            if (options == null)
            {
                return serviceCollection;
            }

            var startupOption = new ConfigurationOptions(serviceCollection);
            options.Invoke(startupOption);

            return serviceCollection;
        }
    }
}
