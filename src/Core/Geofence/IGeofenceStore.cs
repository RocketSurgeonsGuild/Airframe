using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Interface representing a geofence store.
/// </summary>
public interface IGeofenceStore
{
    /// <summary>
    /// Gets an observable sequence of a <see cref="GeofenceRegion"/> change.
    /// </summary>
    /// <param name="id">The region id.</param>
    /// <returns>An observable of region changes.</returns>
    IObservable<GeofenceRegion> Observe(string id);

    /// <summary>
    /// Gets the <see cref="GeofenceRegion"/>.
    /// </summary>
    /// <param name="id">The region id.</param>
    /// <returns>The region.</returns>
    GeofenceRegion Get(string id);

    /// <summary>
    /// Saves the <see cref="GeofenceRegion"/>.
    /// </summary>
    /// <param name="geoRegion">The geo region.</param>
    void Save(GeofenceRegion geoRegion);

    /// <summary>
    /// Saves the list of <see cref="GeofenceRegion"/>.
    /// </summary>
    /// <param name="geoRegion">The geo region.</param>
    void Save(IEnumerable<GeofenceRegion> geoRegion);

    /// <summary>
    /// Removes the <see cref="GeofenceRegion"/>.
    /// </summary>
    /// <param name="id">The region id.</param>
    void Remove(string id);

    /// <summary>
    /// Removes all <see cref="GeofenceRegion"/>.
    /// </summary>
    void RemoveAll();
}