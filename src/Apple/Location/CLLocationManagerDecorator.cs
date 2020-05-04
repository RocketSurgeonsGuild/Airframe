using System;
using System.Threading;
using CoreLocation;
using Foundation;
using JetBrains.Annotations;

namespace Rocket.Surgery.Airframe.Apple
{
    public class CLLocationManagerDecorator : ILocationManager, IDisposable
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