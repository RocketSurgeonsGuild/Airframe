namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Event argument that notifies an authorization state change.
    /// </summary>
    public readonly struct AuthorizationChangedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationChangedEvent"/> struct.
        /// </summary>
        /// <param name="status">The status.</param>
        public AuthorizationChangedEvent(AuthorizationStatus status) => Status = status;

        /// <summary>
        /// Gets or the status of the authorization change.
        /// </summary>
        public AuthorizationStatus Status { get; }
    }
}