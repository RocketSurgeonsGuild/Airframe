using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.Airframe.Core.Tests.Geofence;

internal class GeofenceServiceFixture : ITestFixtureBuilder
{
    public static implicit operator GeofenceService(GeofenceServiceFixture fixture) => fixture.Build();

    private GeofenceService Build() => new GeofenceService();
}