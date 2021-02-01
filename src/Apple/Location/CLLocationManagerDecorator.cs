using System;
using System.Threading;
using CoreLocation;
using Foundation;
using JetBrains.Annotations;

namespace Rocket.Surgery.Airframe.Apple
{
    public class CLLocationManagerDecorator : ICLLocationManager, IDisposable
    {
        private readonly Lazy<CLLocationManager> _locationManager;

        public CLLocationManagerDecorator([CanBeNull] CLLocationManager manager = null)
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
        public void DismissHeadingCalibrationDisplay()
        {
            return default;
        }

        /// <inheritdoc />
        public Boolean IsMonitoringAvailable(Class regionClass)
        {
            return default;
        }

        /// <inheritdoc />
        public void RequestAlwaysAuthorization()
        {
            return default;
        }

        /// <inheritdoc />
        public void RequestLocation()
        {
            return default;
        }

        /// <inheritdoc />
        public void RequestState(CLRegion region)
        {
            return default;
        }

        /// <inheritdoc />
        public void RequestWhenInUseAuthorization()
        {
            return default;
        }

        public void StartMonitoring(CLRegion region, Double desiredAccuracy)
        {
            return default;
        }

        /// <inheritdoc />
        public void StartMonitoring(CLRegion region)
        {
            return default;
        }

        /// <inheritdoc />
        public void StartMonitoringSignificantLocationChanges()
        {
            return default;
        }

        /// <inheritdoc />
        public void StartMonitoringVisits()
        {
            return default;
        }

        /// <inheritdoc />
        public void StartUpdatingHeading()
        {
            return default;
        }

        /// <inheritdoc />
        public void StartUpdatingLocation()
        {
            return default;
        }

        /// <inheritdoc />
        public void StopMonitoring(CLRegion region)
        {
            return default;
        }

        /// <inheritdoc />
        public void StopMonitoringSignificantLocationChanges()
        {
            return default;
        }

        /// <inheritdoc />
        public void StopMonitoringVisits()
        {
            return default;
        }

        /// <inheritdoc />
        public void StopUpdatingHeading()
        {
            return default;
        }

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