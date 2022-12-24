using NSubstitute;
using Rocket.Surgery.Airframe.Shiny.Settings;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using Shiny.Stores;

namespace Airframe.Shiny.Tests.Settings
{
    internal class SettingsProviderFixture : ITestFixtureBuilder
    {
        private IKeyValueStore _settings = Substitute.For<IKeyValueStore>();
        public static implicit operator SettingsProvider(SettingsProviderFixture providerFixture) => providerFixture.Build();

        public SettingsProviderFixture WithSettings(IKeyValueStore settings) => this.With(ref _settings, settings);

        private SettingsProvider Build() => new SettingsProvider(_settings);
    }
}