using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Shiny;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Shiny
{
    /// <summary>
    /// Represents an <see cref="IShinyStartup"/> for application composition.
    /// </summary>
    public abstract class ReactiveShinyStartup : ShinyStartup
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
        public sealed override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            foreach (var serviceDescriptor in services)
            {
                if (!_serviceCollection.Contains(serviceDescriptor))
                {
                    _serviceCollection.Add(serviceDescriptor);
                }
            }

            _serviceCollection.UseMicrosoftDependencyResolver();
            return _serviceCollection.BuildServiceProvider();
        }

        /// <inheritdoc/>
        public sealed override void ConfigureServices(IServiceCollection services, IPlatform platform)
        {
            ConfigureShiny(services);
            ConfigureAppSettings(_serviceCollection, _configurationBuilder);
            ConfigureServices(_serviceCollection);

            RegisterApplicationServices();
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

        private void RegisterApplicationServices()
        {
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI(RegistrationNamespace.XamForms);
            RegisterCoreServices();
        }

        private void ConfigureAppSettings(IServiceCollection services, IConfigurationBuilder configurationBuilder)
        {
            BuildConfiguration(configurationBuilder
               .AddJsonFile("appsettings.json", optional: true)
               .AddJsonFile("appsettings.dev.json", optional: true));

            services.AddSingleton<IConfiguration>(_ => configurationBuilder.AddConfiguration(_configuration).Build());
        }
    }
}