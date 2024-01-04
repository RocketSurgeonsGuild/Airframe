namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents a geographical region.
    /// </summary>
    public class GeoRegion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoRegion"/> class.
        /// </summary>
        /// <param name="identifier">The unique identifier.</param>
        /// <param name="center">The center point of the region.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="notifyOnEntry">A value indicating whether to notify on entry.</param>
        /// <param name="notifyOnExit">A value indicating whether to notify on exit.</param>
        public GeoRegion(string identifier, GeoCoordinate center, double radius, bool notifyOnEntry, bool notifyOnExit)
        {
            Identifier = identifier;
            Center = center;
            NotifyOnEntry = notifyOnEntry;
            NotifyOnExit = notifyOnExit;
            Radius = radius;
        }

        /// <summary>
        /// Gets or sets the region identifier.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        public GeoCoordinate Center { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to notify on entry of the region.
        /// </summary>
        public bool NotifyOnEntry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to notify on exiting the region.
        /// </summary>
        public bool NotifyOnExit { get; set; }

        /// <summary>
        /// Gets or sets the radius of the region.
        /// </summary>
        public double Radius { get; set; }
    }
}