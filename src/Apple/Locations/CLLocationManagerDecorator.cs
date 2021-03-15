using System;
using CoreLocation;
using Foundation;
using JetBrains.Annotations;
using ObjCRuntime;
using Rocket.Surgery.Airframe;

namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Represents a <see cref="CLLocationManager"/>.
    /// </summary>
    public sealed class CLLocationManagerDecorator : ICLLocationManager
    {
        private readonly Lazy<CLLocationManager> _locationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CLLocationManagerDecorator"/> class.
        /// </summary>
        /// <param name="manager">The platform location manager.</param>
        public CLLocationManagerDecorator(CLLocationManager? manager = null)
        {
            _locationManager = new Lazy<CLLocationManager>(() => manager ?? new CLLocationManager());
            _locationManager.Value.AuthorizationChanged += AuthorizationChanged;
            _locationManager.Value.DeferredUpdatesFinished += DeferredUpdatesFinished;
            _locationManager.Value.DidDetermineState += DidDetermineState;
            _locationManager.Value.DidFailRangingBeacons += DidFailRangingBeacons;
            _locationManager.Value.DidRangeBeaconsSatisfyingConstraint += DidRangeBeaconsSatisfyingConstraint;
            _locationManager.Value.DidStartMonitoringForRegion += DidStartMonitoringForRegion;
            _locationManager.Value.DidVisit += DidVisit;
            _locationManager.Value.Failed += Failed;
            _locationManager.Value.LocationUpdatesPaused += LocationUpdatesPaused;
            _locationManager.Value.LocationUpdatesResumed += LocationUpdatesResumed;
            _locationManager.Value.LocationsUpdated += LocationsUpdated;
            _locationManager.Value.MonitoringFailed += MonitoringFailed;
            _locationManager.Value.RegionEntered += RegionEntered;
            _locationManager.Value.RegionLeft += RegionLeft;
            _locationManager.Value.UpdatedHeading += UpdatedHeading;
            _locationManager.Value.UpdatedLocation += UpdatedLocation;
        }

        /// <inheritdoc />
        public event EventHandler<CLAuthorizationChangedEventArgs> AuthorizationChanged;

        /// <inheritdoc />
        public event EventHandler<NSErrorEventArgs> DeferredUpdatesFinished;

        /// <inheritdoc />
        public event EventHandler<CLRegionStateDeterminedEventArgs> DidDetermineState;

        /// <inheritdoc />
        public event EventHandler<CLRegionBeaconsConstraintFailedEventArgs> DidFailRangingBeacons;

        /// <inheritdoc />
        public event EventHandler<CLRegionBeaconsConstraintRangedEventArgs> DidRangeBeaconsSatisfyingConstraint;

        /// <inheritdoc />
        public event EventHandler<CLRegionEventArgs> DidStartMonitoringForRegion;

        /// <inheritdoc />
        public event EventHandler<CLVisitedEventArgs> DidVisit;

        /// <inheritdoc />
        public event EventHandler<NSErrorEventArgs> Failed;

        /// <inheritdoc />
        public event EventHandler LocationUpdatesPaused;

        /// <inheritdoc />
        public event EventHandler LocationUpdatesResumed;

        /// <inheritdoc />
        public event EventHandler<CLLocationsUpdatedEventArgs> LocationsUpdated;

        /// <inheritdoc />
        public event EventHandler<CLRegionErrorEventArgs> MonitoringFailed;

        /// <inheritdoc />
        public event EventHandler<CLRegionEventArgs> RegionEntered;

        /// <inheritdoc />
        public event EventHandler<CLRegionEventArgs> RegionLeft;

        /// <inheritdoc />
        public event EventHandler<CLHeadingUpdatedEventArgs> UpdatedHeading;

        /// <inheritdoc />
        public event EventHandler<CLLocationUpdatedEventArgs> UpdatedLocation;

        /// <inheritdoc />
        public void DismissHeadingCalibrationDisplay() => _locationManager.Value.DismissHeadingCalibrationDisplay();

        /// <inheritdoc />
        public bool IsMonitoringAvailable(Class regionClass) =>

            // TODO: Rodney gets to fix this
            CLLocationManager.IsMonitoringAvailable(regionClass);

        /// <inheritdoc />
        public void RequestAlwaysAuthorization() => _locationManager.Value.RequestAlwaysAuthorization();

        /// <inheritdoc />
        public void RequestLocation() => _locationManager.Value.RequestLocation();

        /// <inheritdoc />
        public void RequestState(CLRegion region) => _locationManager.Value.RequestState(region);

        /// <inheritdoc />
        public void RequestWhenInUseAuthorization() => _locationManager.Value.RequestWhenInUseAuthorization();

        /// <inheritdoc />
        public void StartMonitoring(CLRegion region, double desiredAccuracy) =>

            // TODO: Rodney gets to fix this
#if XAMARIN_MAC
            _locationManager.Value.StartMonitoring(region);
#else
            _locationManager.Value.StartMonitoring(region, desiredAccuracy);
#endif

        /// <inheritdoc />
        public void StartMonitoring(CLRegion region) => _locationManager.Value.StartMonitoring(region);

        /// <inheritdoc />
        public void StartMonitoringSignificantLocationChanges() =>
            _locationManager.Value.StartMonitoringSignificantLocationChanges();

        /// <inheritdoc />
        public void StartMonitoringVisits() => _locationManager.Value.StartMonitoringVisits();

#if XAMARIN_IOS

        /// <inheritdoc />
        public void StartRangingBeacons(CLBeaconRegion region) => _locationManager.Value.StartRangingBeacons(region);

#else
        /// <inheritdoc />
        public void StartRangingBeacons(CLBeaconIdentityConstraint constraint) => _locationManager.Value.StartRangingBeacons(constraint);

#endif

        /// <inheritdoc />
        public void StartUpdatingHeading() => _locationManager.Value.StartUpdatingHeading();

        /// <inheritdoc />
        public void StartUpdatingLocation() => _locationManager.Value.StartUpdatingLocation();

        /// <inheritdoc />
        public void StopMonitoring(CLRegion region) => _locationManager.Value.StopMonitoring(region);

        /// <inheritdoc />
        public void StopMonitoringSignificantLocationChanges() =>
            _locationManager.Value.StopMonitoringSignificantLocationChanges();

        /// <inheritdoc />
        public void StopMonitoringVisits() => _locationManager.Value.StopMonitoringVisits();
#if XAMARIN_IOS

        /// <inheritdoc />
        public void StopRangingBeacons(CLBeaconRegion region) => _locationManager.Value.StopRangingBeacons(region);

#else

        /// <inheritdoc />
        public void StopRangingBeacons(CLBeaconIdentityConstraint constraint) =>
            _locationManager.Value.StopRangingBeacons(constraint);

#endif

        /// <inheritdoc />
        public void StopUpdatingHeading() => _locationManager.Value.StopUpdatingHeading();

        /// <inheritdoc />
        public void StopUpdatingLocation() => _locationManager.Value.StopUpdatingLocation();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _locationManager.Value.AuthorizationChanged -= AuthorizationChanged;
            _locationManager.Value.DeferredUpdatesFinished -= DeferredUpdatesFinished;
            _locationManager.Value.DidDetermineState -= DidDetermineState;
            _locationManager.Value.DidFailRangingBeacons -= DidFailRangingBeacons;
            _locationManager.Value.DidRangeBeaconsSatisfyingConstraint -= DidRangeBeaconsSatisfyingConstraint;
            _locationManager.Value.DidStartMonitoringForRegion -= DidStartMonitoringForRegion;
            _locationManager.Value.DidVisit -= DidVisit;
            _locationManager.Value.Failed -= Failed;
            _locationManager.Value.LocationUpdatesPaused -= LocationUpdatesPaused;
            _locationManager.Value.LocationUpdatesResumed -= LocationUpdatesResumed;
            _locationManager.Value.LocationsUpdated -= LocationsUpdated;
            _locationManager.Value.MonitoringFailed -= MonitoringFailed;
            _locationManager.Value.RegionEntered -= RegionEntered;
            _locationManager.Value.RegionLeft -= RegionLeft;
            _locationManager.Value.UpdatedHeading -= UpdatedHeading;
            _locationManager.Value.UpdatedLocation -= UpdatedLocation;
            _locationManager.Value.Dispose();
        }
    }
}
