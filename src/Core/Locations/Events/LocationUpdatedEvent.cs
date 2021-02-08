namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Notification of a <see cref="GeoLocation"/> update.
    /// </summary>
    public class LocationUpdatedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationUpdatedEvent"/> class.
        /// </summary>
        /// <param name="previous">The previous.</param>
        /// <param name="current">The current.</param>
        public LocationUpdatedEvent(GeoLocation previous, GeoLocation current)
        {
            Previous = previous;
            Current = current;
        }

        /// <summary>
        /// Gets the previous location.
        /// </summary>
        public GeoLocation Previous { get; }

        /// <summary>
        /// Gets the current location.
        /// </summary>
        public GeoLocation Current { get; }
    }
}