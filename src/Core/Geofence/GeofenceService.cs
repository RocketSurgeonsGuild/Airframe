using System;
using System.Collections.Generic;
using Rocket.Surgery.Airframe.Apple;

namespace Rocket.Surgery.Airframe.Geofence
{
    /// <summary>
    /// Represents a geofence service.
    /// </summary>
    public class GeofenceService : IGeofenceService, IGeofenceStore
    {
        /// <inheritdoc/>
        public IObservable<GeoLocation> Location { get; }

        /// <inheritdoc/>
        public IObservable<GeofenceRegion> Entered { get; }

        /// <inheritdoc/>
        public IObservable<GeofenceRegion> Exited { get; }

        /// <inheritdoc/>
        public void StartMonitoring(GeofenceRegion region)
        {
            return default;
        }

        /// <inheritdoc/>
        public void StartMonitoring(IList<GeofenceRegion> regions)
        {
            return default;
        }

        /// <inheritdoc/>
        public void StopMonitoring(string identifier)
        {
            return default;
        }

        /// <inheritdoc/>
        public void StopMonitoring(IList<string> identifiers)
        {
        }

        /// <inheritdoc/>
        public void Save(GeofenceRegion geoRegion)
        {
            return default;
        }

        /// <inheritdoc/>
        public void Save(IEnumerable<GeofenceRegion> geoRegion)
        {
            return default;
        }

        /// <inheritdoc/>
        public void RemoveAll()
        {
            return default;
        }

        /// <inheritdoc/>
        public void Remove(string id)
        {
            return default;
        }

        /// <inheritdoc/>
        public IObservable<GeofenceRegion> Observe(string id)
        {
            return null;
        }

        /// <inheritdoc/>
        public GeofenceRegion Get(string id)
        {
            return null;
        }
    }
}