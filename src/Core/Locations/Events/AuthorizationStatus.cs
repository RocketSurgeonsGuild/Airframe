namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Enumeration of Authorization Status.
    /// </summary>
    public enum AuthorizationStatus : int
    {
        /// <summary>
        /// No Authorization Determined.
        /// </summary>
        NotDetermined = 0,

        /// <summary>
        /// Restricted Authorization.
        /// </summary>
        Restricted = 1,

        /// <summary>
        /// Denied Authorization.
        /// </summary>
        Denied = 2,

        /// <summary>
        /// Always Authorized.
        /// </summary>
        AuthorizedAlways = 3,

        /// <summary>
        /// Authorized Only When In Use.
        /// </summary>
        AuthorizedWhenInUse = 4,
    }
}