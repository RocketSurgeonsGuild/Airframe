using System;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents a latitude and longitude.
    /// </summary>
    public class GeoLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoLocation"/> class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        public GeoLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Gets the latitude.
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Gets the longitude.
        /// </summary>
        public double Longitude { get; }
    }
}