using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Interface representing a service that allows you to monitor <see cref="GeofenceRegion"/>.
    /// </summary>
    public interface IGeofenceService
    {
        /// <summary>
        /// Gets an observable sequence <see cref="Location"/> that have been exited.
        /// </summary>
        IObservable<GeoLocation> Location { get; }

        /// <summary>
        /// Gets an observable sequence <see cref="GeofenceRegion"/> that have been entered.
        /// </summary>
        IObservable<GeofenceRegion> Entered { get; }

        /// <summary>
        /// Gets an observable sequence <see cref="GeofenceRegion"/> that have been exited.
        /// </summary>
        IObservable<GeofenceRegion> Exited { get; }

        /// <summary>
        /// Starts monitoring a region.
        /// </summary>
        /// <param name="region">The region.</param>
        void StartMonitoring(GeofenceRegion region);

        /// <summary>
        /// Starts monitoring multiple regions.
        /// </summary>
        /// <param name="regions">The regions.</param>
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