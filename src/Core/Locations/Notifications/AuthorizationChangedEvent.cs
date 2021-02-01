namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Event argument that notifies an authorization state change.
    /// </summary>
    public readonly struct AuthorizationChangedEvent
    {
        public AuthorizationChangedEvent(AuthorizationStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// Gets or sets the status of the authorization change.
        /// </summary>
        public AuthorizationStatus Status { get; }
    }
}