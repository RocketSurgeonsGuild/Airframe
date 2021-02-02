namespace Rocket.Surgery.Airframe.Locations
{
    /// <summary>
    /// Enumeration of location accuracy.
    /// </summary>
    /// <remarks>https://stackoverflow.com/q/3411629/2088094.</remarks>
    public enum LocationAccuracy
    {
        /// <summary>
        /// Best Accuracy.
        /// </summary>
        AccuracyBest = -1,

        /// <summary>
        /// Accuracy to one hundred meters.
        /// </summary>
        AccuracyHundredMeters = 100,

        /// <summary>
        /// Accuracy to the kilometer.
        /// </summary>
        AccuracyKilometer = 1000,

        /// <summary>
        /// Accuracy to the nearest ten meters.
        /// </summary>
        AccuracyNearestTenMeters = 10,

        /// <summary>
        /// Accuracy to three kilometers.
        /// </summary>
        AccuracyThreeKilometers = 3000,

        /// <summary>
        /// Accuracy for Navigation.
        /// </summary>
        AccuracyBestForNavigation = -2
    }
}