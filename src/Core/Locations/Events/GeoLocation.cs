using System;

namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Represents a latitude and longitude.
    /// </summary>
    public class GeoLocation : IDisposable
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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}