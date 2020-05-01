using System.Collections;
using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.iOS.Notifications
{
    /// <summary>
    /// Notification of updates to locations being monitored.
    /// </summary>
    public class LocationsUpdatedNotification
    {
        /// <summary>
        /// Gets or sets the locations updated by platform service.
        /// </summary>
        public IEnumerable<Location> Locations { get; set; }
    }
}