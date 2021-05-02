using NSubstitute;
using Rocket.Surgery.Airframe.Data;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Airframe.Tests.Data.DuckGo
{
    internal class DuckDuckGoServiceFixture : ITestFixtureBuilder
    {
        private IDuckDuckGoApiClient _client = Substitute.For<IDuckDuckGoApiClient>();
       
        public static implicit operator DuckDuckGoService(DuckDuckGoServiceFixture fixture) => fixture.Build();

        public DuckDuckGoServiceFixture WithClient(IDuckDuckGoApiClient client) => this.With(ref _client, client);
        private DuckDuckGoService Build() => new DuckDuckGoService(_client);
    }
}