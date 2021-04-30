using Rocket.Surgery.Airframe;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Airframe.Tests.Core.Geofence
{
    public class GeofenceServiceTests
    {
        
    }

    internal class GeofenceServiceFixture : ITestFixtureBuilder
    {
        public static implicit operator GeofenceService(GeofenceServiceFixture fixture) => fixture.Build();

        private GeofenceService Build() => new GeofenceService();
    }
}