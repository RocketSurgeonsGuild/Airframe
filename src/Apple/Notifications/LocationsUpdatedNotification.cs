using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Apple.Notifications
{
    public class LocationsUpdatedNotification
    {
        /// <summary>
        /// Gets or sets the locations.
        /// </summary>
        public IEnumerable<Location> Locations { get; set; }
    }
}