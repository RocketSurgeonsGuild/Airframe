using System;
using System.Collections.Generic;
using DynamicData;

namespace Rocket.Surgery.Airframe.Apple
{
    public class GeofenceStore : IGeofenceStore
    {
        private readonly SourceCache<GeofenceRegion, string> _sourceCache = new SourceCache<GeofenceRegion, string>(x => x.Identifier);

        /// <inheritdoc/>
        public IObservable<IChangeSet<GeofenceRegion, string>> Get() => _sourceCache.Connect().RefCount().LimitSizeTo(20);

        /// <inheritdoc/>
        public GeofenceRegion Get(string id)
        {
            var lookup = _sourceCache.Lookup(id);
            if (lookup.HasValue)
            {
                return lookup.Value;
            }

            return default;
        }

        /// <inheritdoc/>
        public void Save(GeofenceRegion geoRegion)
        {
            _sourceCache.AddOrUpdate(geoRegion);
        }

        /// <inheritdoc/>
        public void Save(IEnumerable<GeofenceRegion> geoRegion)
        {
            _sourceCache.AddOrUpdate(geoRegion);
        }

        /// <inheritdoc/>
        public void Remove(string id)
        {
            _sourceCache.Remove(id);
        }

        /// <inheritdoc/>
        public void RemoveAll()
        {
            _sourceCache?.Clear();
        }
    }
}