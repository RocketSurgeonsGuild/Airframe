namespace Rocket.Surgery.Airframe.Apple.Notifications
{
    /// <summary>
    /// Notification of a <see cref="Location"/> update.
    /// </summary>
    public class LocationUpdatedNotification
    {
        /// <summary>
        /// Gets or sets the previous location.
        /// </summary>
        public Location Previous { get; set; }

        /// <summary>
        /// Gets or sets the current location.
        /// </summary>
        public Location Current { get; set; }
    }
}