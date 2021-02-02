using System;
using Rocket.Surgery.Airframe.Geofence;

namespace Rocket.Surgery.Airframe.Locations.Events
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
        public Exception Error { get; }

        /// <summary>
        /// Gets or sets the geo region.
        /// </summary>
        public GeoRegion Region { get; }
    }
}