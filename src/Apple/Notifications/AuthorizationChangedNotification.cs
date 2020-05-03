namespace Rocket.Surgery.Airframe.Apple.Notifications
{
    /// <summary>
    /// Event argument that notifies an authorization state change.
    /// </summary>
    public class AuthorizationChangedNotification
    {
        /// <summary>
        /// Gets or sets the status of the authorization change.
        /// </summary>
        public AuthorizationStatus Status { get; set; }
    }
}