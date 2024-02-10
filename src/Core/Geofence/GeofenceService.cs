using System;
using System.Collections.Generic;
using DynamicData;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Represents a geofence service.
/// </summary>
public class GeofenceService : IGeofenceService, IGeofenceStore
{
    private readonly SourceCache<GeofenceRegion, string> _store = new SourceCache<GeofenceRegion, string>(region => region.Identifier);

    /// <inheritdoc/>
    public IObservable<GeoCoordinate> Location { get; }

    /// <inheritdoc/>
    public IObservable<GeofenceRegion> Entered { get; }

    /// <inheritdoc/>
    public IObservable<GeofenceRegion> Exited { get; }

    /// <inheritdoc/>
    public void StartMonitoring(GeofenceRegion region) => _store.AddOrUpdate(region);

    /// <inheritdoc/>
    public void StartMonitoring(IList<GeofenceRegion> regions) => _store.AddOrUpdate(regions);

    /// <inheritdoc/>
    public void StopMonitoring(string identifier) => _store.Remove(identifier);

    /// <inheritdoc/>
    public void StopMonitoring(IList<string> identifiers) => _store.Remove(identifiers);

    /// <inheritdoc/>
    public void Save(GeofenceRegion geoRegion) => _store.AddOrUpdate(geoRegion);

    /// <inheritdoc/>
    public void Save(IEnumerable<GeofenceRegion> geoRegion) => _store.AddOrUpdate(geoRegion);

    /// <inheritdoc/>
    public void RemoveAll() => _store.Clear();

    /// <inheritdoc/>
    public void Remove(string id) => _store.Remove(id);

    /// <inheritdoc/>
    public IObservable<GeofenceRegion> Observe(string id) => _store.WatchValue(id);

    /// <inheritdoc/>
    public GeofenceRegion Get(string id)
    {
        var optional = _store.Lookup(id);
        if (optional.HasValue)
        {
            return optional.Value;
        }

        return GeofenceRegion.Default;
    }
}