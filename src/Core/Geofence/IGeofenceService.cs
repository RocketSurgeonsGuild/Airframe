using System;
using System.Collections.Generic;
using Rocket.Surgery.Airframe.Apple;

namespace Rocket.Surgery.Airframe.Geofence
{
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
        /// <param name="region"></param>
        void StartMonitoring(GeofenceRegion region);

        /// <summary>
        /// Starts monitoring multiple regions.
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

    public class GeofenceService : IGeofenceService, IGeofenceStore
    {
        public IObservable<GeoLocation> Location { get; }
        public IObservable<GeofenceRegion> Entered { get; }
        public IObservable<GeofenceRegion> Exited { get; }
        public void StartMonitoring(GeofenceRegion region)
        {
            return default;
        }

        public void StartMonitoring(IList<GeofenceRegion> regions)
        {
            return default;
        }

        public void StopMonitoring(string identifier)
        {
            return default;
        }

        public IObservable<IChangeSet<GeofenceRegion, string>> Get()
        {
            return default;
        }

        public void Save(GeofenceRegion geoRegion)
        {
            return default;
        }

        public void Save(IEnumerable<GeofenceRegion> geoRegion)
        {
            return default;
        }

        public void RemoveAll()
        {
            return default;
        }

        public void Remove(string id)
        {
            return default;
        }

        public GeofenceRegion Get(string id)
        {
            return null;
        }
    }
}