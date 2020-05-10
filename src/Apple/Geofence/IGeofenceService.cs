using System.Collections.Generic;
using JetBrains.Annotations;

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

    public class GeofenceService : IGeofenceService
    {
        private readonly ICoreLocationService _coreLocationService;
        private readonly IGeofenceStore _geofenceStore;

        public GeofenceService([NotNull] ICoreLocationService coreLocationService, [NotNull] IGeofenceStore geofenceStore)
        {
            _coreLocationService = coreLocationService;
            _geofenceStore = geofenceStore;
        }

        public IGeofenceStore Store { get; }
        public Location LastLocation { get; set; }
        public void StartMonitoring(GeofenceRegion region)
        {
            throw new System.NotImplementedException();
        }

        public void StartMonitoring(IList<GeofenceRegion> regions)
        {
            throw new System.NotImplementedException();
        }

        public void StopMonitoring(string identifier)
        {
            throw new System.NotImplementedException();
        }

        public void StopMonitoring(IList<string> identifiers)
        {
            throw new System.NotImplementedException();
        }
    }
}