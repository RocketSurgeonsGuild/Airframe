namespace Rocket.Surgery.Airframe;

/// <summary>
/// Represents the current state of the platform service configuration.
/// </summary>
public class MonitorState
{
    /// <summary>
    /// Gets or sets a value indicating whether to allow background location updates.
    /// </summary>
    public bool AllowingBackgroundLocationUpdates { get; set; }

    /// <summary>
    /// Gets or sets the desired accuracy.
    /// </summary>
    public double DesiredAccuracy { get; set; }

    /// <summary>
    /// Gets or sets the distance filter.
    /// </summary>
    public double DistanceFilter { get; set; }

    /// <summary>
    /// Gets or sets the current location.
    /// </summary>
    public GeoLocation GeoLocation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether location services are enabled.
    /// </summary>
    public bool LocationServicesEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the location services are enabled.
    /// </summary>
    public bool MaximumRegionMonitoringDistance { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to pause location updates automatically.
    /// </summary>
    public bool PausesLocationUpdatesAutomatically { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether region monitoring is available.
    /// </summary>
    public bool RegionMonitoringAvailable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether region monitoring is enabled.
    /// </summary>
    public bool RegionMonitoringEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show background location indicator.
    /// </summary>
    public bool ShowsBackgroundLocationIndicator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether significant location changed monitoring is available.
    /// </summary>
    public bool SignificantLocationChangeMonitoringAvailable { get; set; }

    /// <summary>
    /// Gets or sets the maximum time interval.
    /// </summary>
    public double MaxTimeInterval { get; set; }

    /// <summary>
    /// Gets or sets the authorization status.
    /// </summary>
    public AuthorizationStatus Status { get; set; }
}