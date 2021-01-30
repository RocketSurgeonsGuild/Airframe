namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Notification of a <see cref="Location"/> update.
    /// </summary>
    public class LocationUpdatedEvent
    {
        public LocationUpdatedEvent(Location previous, Location current)
        {
            Previous = previous;
            Current = current;
        }

        /// <summary>
        /// Gets or sets the previous location.
        /// </summary>
        public Location Previous { get; }

        /// <summary>
        /// Gets or sets the current location.
        /// </summary>
        public Location Current { get; }
    }
}