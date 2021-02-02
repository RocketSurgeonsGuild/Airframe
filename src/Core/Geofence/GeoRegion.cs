using System;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents a geographical region.
    /// </summary>
    public class GeoRegion : IDisposable
    {
        /// <summary>
        /// Gets or sets the region identifier.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        public GeoLocation Center { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to notify on entry of the region.
        /// </summary>
        public bool NotifyOnEntry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to notify on exiting the region.
        /// </summary>
        public bool NotifyOnExit { get; set; }

        /// <summary>
        /// Gets or sets the radius of the region.
        /// </summary>
        public double Radius { get; set; }

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