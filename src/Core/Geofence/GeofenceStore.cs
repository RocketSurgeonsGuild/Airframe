using System;
using System.Collections.Generic;
using DynamicData;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents a store for <see cref="GeofenceRegion"/> being monitored.
    /// </summary>
    public class GeofenceStore : IGeofenceStore
    {
        private readonly SourceCache<GeofenceRegion, string> _sourceCache = new SourceCache<GeofenceRegion, string>(x => x.Identifier);

        /// <inheritdoc/>
        public IObservable<GeofenceRegion> Observe(string id) => _sourceCache.WatchValue(id);

        /// <inheritdoc/>
        public GeofenceRegion Get(string id)
        {
            var lookup = _sourceCache.Lookup(id);
            return lookup.HasValue ? lookup.Value : GeofenceRegion.Default;
        }

        /// <inheritdoc/>
        public void Save(GeofenceRegion geoRegion) => _sourceCache.AddOrUpdate(geoRegion);

        /// <inheritdoc/>
        public void Save(IEnumerable<GeofenceRegion> geoRegion) => _sourceCache.AddOrUpdate(geoRegion);

        /// <inheritdoc/>
        public void Remove(string id) => _sourceCache.Remove(id);

        /// <inheritdoc/>
        public void RemoveAll() => _sourceCache.Clear();
    }
}