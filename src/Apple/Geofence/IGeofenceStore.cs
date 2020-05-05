using System;
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
}