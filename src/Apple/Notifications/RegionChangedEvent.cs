using Rocket.Surgery.Airframe.Apple.Notifications;

namespace Rocket.Surgery.Airframe.Apple
{
    public class RegionChangedEvent
    {
        public RegionChangedEvent(GeoRegion region, RegionState? state = null)
        {
            Region = region;
            State = state;
        }

        public GeoRegion Region { get; }

        public RegionState? State { get; }
    }
}