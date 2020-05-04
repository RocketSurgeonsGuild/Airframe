using Rocket.Surgery.Airframe.Apple.Notifications;

namespace Rocket.Surgery.Airframe.Apple
{
    public class RegionChangedNotification
    {
        public GeoRegion Region { get; set; }

        public RegionState? State { get; set; }
    }
}