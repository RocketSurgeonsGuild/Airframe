using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Shiny;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Composition.Shiny
{
    /// <summary>
    /// Represents an <see cref="IShinyStartup"/> for application composition.
    /// </summary>
    public abstract class ReactiveShinyStartup : IShinyStartup
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IConfigurationBuilder _configurationBuilder = new ConfigurationBuilder();
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveShinyStartup"/> class.
        /// </summary>
        protected ReactiveShinyStartup()
            : this(new ServiceCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveShinyStartup"/> class.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        protected ReactiveShinyStartup(IServiceCollection serviceCollection)
            : this(serviceCollection, new ConfigurationBuilder().Build()) => _serviceCollection = serviceCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveShinyStartup"/> class.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        protected ReactiveShinyStartup(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            _serviceCollection = serviceCollection;
            _configuration = configuration;
        }

        /// <inheritdoc/>
        void IShinyStartup.ConfigureApp(IServiceProvider serviceProvider) => ConfigureApp(serviceProvider);

        /// <inheritdoc/>
        IServiceProvider IShinyStartup.CreateServiceProvider(IServiceCollection services)
        {
            foreach (var service in services)
            {
                if (!_serviceCollection.Contains(service))
                {
                    _serviceCollection.Add(service);
                }
            }

            _serviceCollection.UseMicrosoftDependencyResolver();

            return _serviceCollection.BuildServiceProvider();
        }

        /// <inheritdoc/>
        void IShinyStartup.ConfigureServices(IServiceCollection services)
        {
            ConfigureShiny(services);
            ConfigureAppSettings(services, _configurationBuilder);
            ConfigureServices(_serviceCollection);
            RegisterCoreServices(_serviceCollection);
        }

        /// <summary>
        /// Configure the application with the given service provider.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        protected virtual void ConfigureApp(IServiceProvider provider)
        {
        }

        /// <summary>
        /// Configure services with the given service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        protected abstract void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Configure <see cref="Shiny"/> with the given service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        protected abstract void ConfigureShiny(IServiceCollection services);

        /// <summary>
        /// Register core services.
        /// </summary>
        protected virtual void RegisterCoreServices()
        {
        }

        /// <summary>
        /// Build your custom <see cref="IConfigurationBuilder"/> into the composition root.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        protected virtual void BuildConfiguration(IConfigurationBuilder configurationBuilder)
        {
        }

        private void RegisterCoreServices(IServiceCollection serviceCollection)
        {
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI(RegistrationNamespace.XamForms);
            RegisterCoreServices();
        }

        private void ConfigureAppSettings(IServiceCollection services, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.dev.json", optional: true);

            BuildConfiguration(configurationBuilder);

            services.AddSingleton<IConfiguration>(_ => configurationBuilder.AddConfiguration(_configuration).Build());
        }
    }
}