using System;
using System.Collections.Generic;
using DynamicData;
using Rocket.Surgery.Airframe.Apple.Notifications;

namespace Rocket.Surgery.Airframe.Apple
{
    public interface IGeofenceStore
    {
        IObservable<IChangeSet<GeofenceRegion, string>> Get();

        GeofenceRegion Get(string id);

        void Save(GeofenceRegion geoRegion);

        void Save(IEnumerable<GeofenceRegion> geoRegion);

        void Remove(string id);

        void RemoveAll();
    }

    public class GeofenceStore : IGeofenceStore
    {
        SourceCache<GeofenceRegion, string> _sourceCache = new SourceCache<GeofenceRegion, string>(x => x.Identifier);

        public GeofenceStore()
        {
            
        }

        public IObservable<IChangeSet<GeofenceRegion, string>> Get() => _sourceCache.Connect().RefCount().LimitSizeTo(20);

        public GeofenceRegion Get(string id)
        {
            var lookup = _sourceCache.Lookup(id);
            if (lookup.HasValue)
            {
                return lookup.Value;
            }

            return default;
        }

        public void Save(GeofenceRegion geoRegion)
        {
            _sourceCache.AddOrUpdate(geoRegion);
        }

        public void Save(IEnumerable<GeofenceRegion> geoRegion)
        {
            _sourceCache.AddOrUpdate(geoRegion);
        }

        public void Remove(string id)
        {
            _sourceCache.Remove(id);
        }

        public void RemoveAll()
        {
            if (_sourceCache != null) _sourceCache.Clear();
        }
    }
}