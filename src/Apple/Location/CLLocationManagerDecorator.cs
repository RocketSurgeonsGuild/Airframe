using System;
using System.Threading;
using CoreLocation;
using Foundation;
using JetBrains.Annotations;

namespace Rocket.Surgery.Airframe.Apple
{
    public class CLLocationManagerDecorator : ILocationManager, IDisposable
    {
        private readonly Lazy<CLLocationManager> LocationManager;

        public CLLocationManagerDecorator([CanBeNull] CLLocationManager manager = null)
        {
            LocationManager = new Lazy<CLLocationManager>(() => manager ?? new CLLocationManager());
            LocationManager.Value.AuthorizationChanged += AuthorizationChanged;
            LocationManager.Value.DeferredUpdatesFinished += DeferredUpdatesFinished;
            LocationManager.Value.DidDetermineState += DidDetermineState;
            LocationManager.Value.DidFailRangingBeacons += DidFailRangingBeacons;
            LocationManager.Value.DidRangeBeacons += DidRangeBeacons;
            LocationManager.Value.DidRangeBeaconsSatisfyingConstraint += DidRangeBeaconsSatisfyingConstraint;
            LocationManager.Value.DidStartMonitoringForRegion += DidStartMonitoringForRegion;
            LocationManager.Value.DidVisit += DidVisit;
            LocationManager.Value.Failed += Failed;
            LocationManager.Value.LocationUpdatesPaused += LocationUpdatesPaused;
            LocationManager.Value.LocationUpdatesResumed += LocationUpdatesResumed;
            LocationManager.Value.LocationsUpdated += LocationsUpdated;
            LocationManager.Value.MonitoringFailed += MonitoringFailed;
            LocationManager.Value.RangingBeaconsDidFailForRegion += RangingBeaconsDidFailForRegion;
            LocationManager.Value.RegionEntered += RegionEntered;
            LocationManager.Value.RegionLeft += RegionLeft;
            LocationManager.Value.UpdatedHeading += UpdatedHeading;
            LocationManager.Value.UpdatedLocation += UpdatedLocation;
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
                LocationManager.Value.AuthorizationChanged -= AuthorizationChanged;
                LocationManager.Value.DeferredUpdatesFinished -= DeferredUpdatesFinished;
                LocationManager.Value.DidDetermineState -= DidDetermineState;
                LocationManager.Value.DidFailRangingBeacons -= DidFailRangingBeacons;
                LocationManager.Value.DidRangeBeacons -= DidRangeBeacons;
                LocationManager.Value.DidRangeBeaconsSatisfyingConstraint -= DidRangeBeaconsSatisfyingConstraint;
                LocationManager.Value.DidStartMonitoringForRegion -= DidStartMonitoringForRegion;
                LocationManager.Value.DidVisit -= DidVisit;
                LocationManager.Value.Failed -= Failed;
                LocationManager.Value.LocationUpdatesPaused -= LocationUpdatesPaused;
                LocationManager.Value.LocationUpdatesResumed -= LocationUpdatesResumed;
                LocationManager.Value.LocationsUpdated -= LocationsUpdated;
                LocationManager.Value.MonitoringFailed -= MonitoringFailed;
                LocationManager.Value.RangingBeaconsDidFailForRegion -= RangingBeaconsDidFailForRegion;
                LocationManager.Value.RegionEntered -= RegionEntered;
                LocationManager.Value.RegionLeft -= RegionLeft;
                LocationManager.Value.UpdatedHeading -= UpdatedHeading;
                LocationManager.Value.UpdatedLocation -= UpdatedLocation;
                LocationManager.Value.Dispose();
            }
        }
    }
}