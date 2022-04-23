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
        /// Configures the app settings for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection with ReactiveUI dependencies registered.</returns>
        public static IServiceCollection ConfigureAppSettings(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true)
               .AddJsonFile("appsettings.dev.json", optional: true);

            serviceCollection.AddSingleton<IConfiguration>(_ => builder.AddConfiguration(configuration).Build());

            return serviceCollection;
        }

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
    }
}
