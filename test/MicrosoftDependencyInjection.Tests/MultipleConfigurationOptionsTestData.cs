using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.FlatOptions;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests
{
    internal class MultipleConfigurationOptionsTestData : TestClassData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleConfigurationOptionsTestData"/> class.
        /// </summary>
        public MultipleConfigurationOptionsTestData() => _buildServiceProvider = new ServiceCollection()
           .ConfigureSettings(
                configuration => configuration
                   .AddJsonFile("DefaultOptions/defaultoptions.json", optional: false)
                   .AddJsonFile("FlatOptions/flatsettings.json", optional: false)
                   .AddJsonFile("ComplexOptions/multilevelsettings.json", optional: false),
                options => options
                   .ConfigureOption<FlatSettings>()
                   .ConfigureSection<TransientFaultHandlingOptions>()
                   .ConfigureOption<LevelSettings>())
           .BuildServiceProvider();

        /// <inheritdoc/>
        protected override IEnumerator<object[]> Enumerator()
        {
            yield return new object[] { _buildServiceProvider };
        }

        private readonly ServiceProvider _buildServiceProvider;
    }
}