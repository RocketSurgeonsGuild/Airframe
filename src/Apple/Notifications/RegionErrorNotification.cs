using System;
using Rocket.Surgery.Airframe.Apple.Notifications;

namespace Rocket.Surgery.Airframe.Apple
{
    public class RegionErrorNotification
    {
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the geo region.
        /// </summary>
        public GeoRegion Region { get; set; }
    }
}