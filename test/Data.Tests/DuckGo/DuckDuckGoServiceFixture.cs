using NSubstitute;
using Rocket.Surgery.Airframe.Data.DuckDuckGo;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.Airframe.Data.Tests.DuckGo
{
    internal class DuckDuckGoServiceFixture : ITestFixtureBuilder
    {
        public static implicit operator DuckDuckGoService(DuckDuckGoServiceFixture fixture) => fixture.Build();

        public DuckDuckGoServiceFixture WithClient(IDuckDuckGoApiClient client) => this.With(ref _client, client);

        private DuckDuckGoService Build() => new(_client);

        private IDuckDuckGoApiClient _client = Substitute.For<IDuckDuckGoApiClient>();
    }
}