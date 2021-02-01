namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Notification of a <see cref="GeoLocation"/> update.
    /// </summary>
    public class LocationUpdatedEvent
    {
        public LocationUpdatedEvent(GeoLocation previous, GeoLocation current)
        {
            Previous = previous;
            Current = current;
        }

        /// <summary>
        /// Gets or sets the previous location.
        /// </summary>
        public GeoLocation Previous { get; }

        /// <summary>
        /// Gets or sets the current location.
        /// </summary>
        public GeoLocation Current { get; }
    }
}