using System;
using Rocket.Surgery.Airframe.Geofence;

namespace Rocket.Surgery.Airframe.Apple
{
    public class RegionErrorEvent
    {
        public RegionErrorEvent(NSError error, GeoRegion region)
        {
            Error = error;
            Region = region;
        }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public NSError Error { get; }

        /// <summary>
        /// Gets or sets the geo region.
        /// </summary>
        public GeoRegion Region { get; }
    }
}