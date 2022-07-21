using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Airframe.Shiny;

namespace Airframe.Shiny.Tests
{
    internal class TestStartup : ReactiveShinyStartup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveShinyStartup"/> class.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        public TestStartup(IServiceCollection serviceCollection, IConfiguration configuration)
            : base(serviceCollection, configuration) { }

        protected override void ConfigureServices(IServiceCollection services) { }

        protected override void ConfigureShiny(IServiceCollection services) { }
    }
}
