using NSubstitute;
using Rocket.Surgery.Airframe.Shiny.Settings;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using Shiny.Settings;

namespace Airframe.Tests.Shiny.Settings
{
    internal class SettingsProviderFixture : ITestFixtureBuilder
    {
        private ISettings _settings = Substitute.For<ISettings>();
        public static implicit operator SettingsProvider(SettingsProviderFixture providerFixture) => providerFixture.Build();

        public SettingsProviderFixture WithSettings(ISettings settings) => this.With(ref _settings, settings);
        
        private SettingsProvider Build() => new SettingsProvider(_settings);
    }
}