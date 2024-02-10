using Rocket.Surgery.Extensions.Testing.Fixtures;
using System.Linq;

namespace Rocket.Surgery.Airframe.Core.Tests.Connectivity;

internal sealed class NetworkStateChangedEventFixture : ITestFixtureBuilder
{
    public static implicit operator NetworkStateChangedEvent(NetworkStateChangedEventFixture fixture) => fixture.Build();

    public NetworkStateChangedEventFixture WithDefault() => WithDefaultAccess().WithDefaultProfiles();

    public NetworkStateChangedEventFixture WithDefaultAccess() => this.With(ref _networkAccess, NetworkAccess.Internet);

    public NetworkStateChangedEventFixture WithDefaultProfiles() => WithProfiles(
        NetworkConnection.Bluetooth,
        NetworkConnection.Ethernet,
        NetworkConnection.WiFi
    );

    public NetworkStateChangedEventFixture WithProfiles(params NetworkConnection[] profiles) => this.With(ref _profiles, profiles);

    private NetworkStateChangedEvent Build() => new NetworkStateChangedEvent(_networkAccess, _profiles.ToArray());

    private NetworkAccess _networkAccess = NetworkAccess.Internet;
    private NetworkConnection[] _profiles = { NetworkConnection.WiFi };
}