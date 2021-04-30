using System.Collections.Generic;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents a <see cref="GeoLocation"/> update event.
    /// </summary>
    public class LocationsUpdatedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsUpdatedEvent"/> class.
        /// </summary>
        /// <param name="locations">The locations.</param>
        public LocationsUpdatedEvent(IEnumerable<GeoLocation> locations) => Locations = locations;

        /// <summary>
        /// Gets the locations.
        /// </summary>
        public IEnumerable<GeoLocation> Locations { get; }
    }
}