using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Apple
{
    public class LocationsUpdatedEvent
    {
        public LocationsUpdatedEvent(IEnumerable<GeoLocation> locations)
        {
            Locations = locations;
        }

        /// <summary>
        /// Gets or sets the locations.
        /// </summary>
        public IEnumerable<GeoLocation> Locations { get; }
    }
}