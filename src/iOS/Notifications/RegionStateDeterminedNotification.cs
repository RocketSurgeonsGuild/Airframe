namespace Rocket.Surgery.Airframe.iOS.Notifications
{
    public class RegionStateDeterminedNotification
    {
        /// <summary>
        ///  Gets or sets the region.
        /// </summary>
        public GeoRegion Region { get; set; }

        /// <summary>
        /// Gets or sets the region state.
        /// </summary>
        public RegionState State { get; set; }
    }
}