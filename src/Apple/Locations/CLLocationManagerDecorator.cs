using System;
using System.Threading;
using CoreLocation;
using Foundation;
using JetBrains.Annotations;
using Rocket.Surgery.Airframe;

namespace Rocket.Surgery.Airframe.Apple
{
    public class CLLocationManagerDecorator : ICLLocationManager
    {
        private readonly Lazy<CLLocationManager> _locationManager;

        public CLLocationManagerDecorator([CanBeNull] CLLocationManager manager = default)
        {
            _locationManager = new Lazy<CLLocationManager>(() => manager ?? new CLLocationManager());
            _locationManager.Value.AuthorizationChanged += AuthorizationChanged;
            _locationManager.Value.DeferredUpdatesFinished += DeferredUpdatesFinished;
            _locationManager.Value.DidDetermineState += DidDetermineState;
            _locationManager.Value.DidFailRangingBeacons += DidFailRangingBeacons;
            _locationManager.Value.DidRangeBeacons += DidRangeBeacons;
            _locationManager.Value.DidRangeBeaconsSatisfyingConstraint += DidRangeBeaconsSatisfyingConstraint;
            _locationManager.Value.DidStartMonitoringForRegion += DidStartMonitoringForRegion;
            _locationManager.Value.DidVisit += DidVisit;
            _locationManager.Value.Failed += Failed;
            _locationManager.Value.LocationUpdatesPaused += LocationUpdatesPaused;
            _locationManager.Value.LocationUpdatesResumed += LocationUpdatesResumed;
            _locationManager.Value.LocationsUpdated += LocationsUpdated;
            _locationManager.Value.MonitoringFailed += MonitoringFailed;
            _locationManager.Value.RangingBeaconsDidFailForRegion += RangingBeaconsDidFailForRegion;
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
        public event EventHandler<CLRegionBeaconsRangedEventArgs> DidRangeBeacons;

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
        public event EventHandler<CLRegionBeaconsFailedEventArgs> RangingBeaconsDidFailForRegion;

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
            _locationManager.Value.IsMonitoringAvailable(regionClass);

        /// <inheritdoc />
        public void RequestAlwaysAuthorization() => _locationManager.Value.RequestAlwaysAuthorization();

        /// <inheritdoc />
        public void RequestLocation() => _locationManager.Value.RequestLocation();

        /// <inheritdoc />
        public void RequestState(CLRegion region) => _locationManager.Value.RequestState(region);

        /// <inheritdoc />
        public void RequestWhenInUseAuthorization() => _locationManager.Value.RequestWhenInUseAuthorization();

        /// <inheritdoc />
        public void StartMonitoring(CLRegion region, Double desiredAccuracy) =>
            _locationManager.Value.StartMonitoring(region, desiredAccuracy);

        /// <inheritdoc />
        public void StartMonitoring(CLRegion region) => _locationManager.Value.StartMonitoring(region);

        /// <inheritdoc />
        public void StartMonitoringSignificantLocationChanges() =>
            _locationManager.Value.StartMonitoringSignificantLocationChanges();

        /// <inheritdoc />
        public void StartMonitoringVisits() => _locationManager.Value.StartMonitoringVisits();

        public void StartRangingBeacons(CLBeaconRegion region)
        {
            return default;
        }

        public void StartRangingBeacons(CLBeaconIdentityConstraint constraint)
        {
            return default;
        }

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

        public void StopRangingBeacons(CLBeaconRegion region)
        {
            return default;
        }

        public void StopRangingBeacons(CLBeaconIdentityConstraint constraint)
        {
            return default;
        }

        /// <inheritdoc />
        public void StopUpdatingHeading() => _locationManager.Value.StopUpdatingHeading();

        /// <inheritdoc />
        public void StopUpdatingLocation() => _locationManager.Value.StopUpdatingLocation();

        /// <inheritdoc />
        public void StartMonitoring(CLRegion region, Double desiredAccuracy) =>
            _locationManager.Value.StartMonitorying(region, desiredAccuracy);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _locationManager.Value.AuthorizationChanged -= AuthorizationChanged;
                _locationManager.Value.DeferredUpdatesFinished -= DeferredUpdatesFinished;
                _locationManager.Value.DidDetermineState -= DidDetermineState;
                _locationManager.Value.DidFailRangingBeacons -= DidFailRangingBeacons;
                _locationManager.Value.DidRangeBeacons -= DidRangeBeacons;
                _locationManager.Value.DidRangeBeaconsSatisfyingConstraint -= DidRangeBeaconsSatisfyingConstraint;
                _locationManager.Value.DidStartMonitoringForRegion -= DidStartMonitoringForRegion;
                _locationManager.Value.DidVisit -= DidVisit;
                _locationManager.Value.Failed -= Failed;
                _locationManager.Value.LocationUpdatesPaused -= LocationUpdatesPaused;
                _locationManager.Value.LocationUpdatesResumed -= LocationUpdatesResumed;
                _locationManager.Value.LocationsUpdated -= LocationsUpdated;
                _locationManager.Value.MonitoringFailed -= MonitoringFailed;
                _locationManager.Value.RangingBeaconsDidFailForRegion -= RangingBeaconsDidFailForRegion;
                _locationManager.Value.RegionEntered -= RegionEntered;
                _locationManager.Value.RegionLeft -= RegionLeft;
                _locationManager.Value.UpdatedHeading -= UpdatedHeading;
                _locationManager.Value.UpdatedLocation -= UpdatedLocation;
                _locationManager.Value.Dispose();
            }
        }
    }
}