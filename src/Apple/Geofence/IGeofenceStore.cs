using DynamicData;
using Rocket.Surgery.Airframe.Apple.Notifications;

namespace Rocket.Surgery.Airframe.Apple.Geofence
{
    public interface IGeofenceStore
    {
        IObservableCache<string, GeoRegion> Regions { get; }
    }
}