using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Apple
{
    public interface IGeofenceService
    {
        IGeofenceStore Store { get; }

        Location LastLocation { get; set; }

        void StartMonitoring(GeofenceRegion region);

        /// <summary>
        /// Starts monitoring multiple regions
        /// </summary>
        /// <param name="regions"></param>
        void StartMonitoring(IList<GeofenceRegion> regions);

        /// <summary>
        /// Stops monitoring the specified regions.
        /// </summary>
        /// <param name="identifier">The region identifier.</param>
        void StopMonitoring(string identifier);

        /// <summary>
        /// Stops monitoring multiple regions.
        /// </summary>
        /// <param name="identifiers">The identifiers for the regions.</param>
        void StopMonitoring(IList<string> identifiers);
    }
}