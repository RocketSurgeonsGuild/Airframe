namespace Rocket.Surgery.Airframe.Apple.Notifications
{
    public class RegionChangedNotification
    {
        public GeoRegion Region { get; set; }

        public RegionState? State { get; set; }
    }
}