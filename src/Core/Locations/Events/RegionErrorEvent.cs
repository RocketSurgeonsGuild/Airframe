using System;

namespace Rocket.Surgery.Airframe.Events
{
    /// <summary>
    /// Represents a error for a region.
    /// </summary>
    public class RegionErrorEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionErrorEvent"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="region">The region.</param>
        public RegionErrorEvent(Exception error, GeoRegion region)
        {
            Error = error;
            Region = region;
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Error { get; }

        /// <summary>
        /// Gets the geo region.
        /// </summary>
        public GeoRegion Region { get; }
    }
}