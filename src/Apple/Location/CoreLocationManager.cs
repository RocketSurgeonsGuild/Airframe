using System;
using System.Reactive;
using System.Reactive.Linq;
using CoreLocation;
using Foundation;
using Rocket.Surgery.Airframe.Apple.Notifications;

namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Implementation of <see cref="ICoreLocationManager"/> on top of <see cref="CLLocationManager" />.
    /// </summary>
    public class CoreLocationManager : ICoreLocationManager
    {
        private readonly Lazy<CLLocationManager> _locationManager = new Lazy<CLLocationManager>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreLocationManager"/> class.
        /// </summary>
        public CoreLocationManager()
        {
            AuthorizationChanged =
                Observable
                    .FromEvent<EventHandler<CLAuthorizationChangedEventArgs>, CLAuthorizationChangedEventArgs>(
                        x => _locationManager.Value.AuthorizationChanged += x,
                        x => _locationManager.Value.AuthorizationChanged -= x)
                    .Select(x => new AuthorizationChangedNotification());

            DeferredUpdatesFinished = Observable.FromEvent<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(
                    x => _locationManager.Value.DeferredUpdatesFinished += x,
                    x => _locationManager.Value.DeferredUpdatesFinished -= x)
                .Select(x => new ErrorNotification());

            DidDetermineState =
                Observable
                    .FromEvent<EventHandler<CLRegionStateDeterminedEventArgs>, CLRegionStateDeterminedEventArgs>(
                        x => _locationManager.Value.DidDetermineState += x,
                        x => _locationManager.Value.DidDetermineState -= x)
                    .Select(x => new RegionChangedNotification());

            DidFailRangingBeacons =
                Observable
                    .FromEvent<EventHandler<CLRegionBeaconsConstraintFailedEventArgs>,
                        CLRegionBeaconsConstraintFailedEventArgs>(
                        x => _locationManager.Value.DidFailRangingBeacons += x,
                        x => _locationManager.Value.DidFailRangingBeacons -= x)
                    .Select(x => new RegionBeaconsConstraintFailedNotification());

            DidRangeBeacons =
                Observable.FromEvent<EventHandler<CLRegionBeaconsRangedEventArgs>, CLRegionBeaconsRangedEventArgs>(
                        x => _locationManager.Value.DidRangeBeacons += x,
                        x => _locationManager.Value.DidRangeBeacons -= x)
                    .Select(x => new RegionBeaconRangedNotification());

            DidRangeBeaconsSatisfyingConstraint =
                Observable
                    .FromEvent<EventHandler<CLRegionBeaconsConstraintRangedEventArgs>, CLRegionBeaconsConstraintRangedEventArgs>(
                        x => _locationManager.Value.DidRangeBeaconsSatisfyingConstraint += x,
                        x => _locationManager.Value.DidRangeBeaconsSatisfyingConstraint -= x)
                    .Select(x => new RegionBeaconsConstraintRangedNotification());

            DidStartMonitoringForRegion =
                Observable
                    .FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        x => _locationManager.Value.DidStartMonitoringForRegion += x,
                        x => _locationManager.Value.DidStartMonitoringForRegion -= x)
                    .Select(x => new RegionChangedNotification());

            DidVisit =
                Observable.FromEvent<EventHandler<CLVisitedEventArgs>, CLVisitedEventArgs>(
                        x => _locationManager.Value.DidVisit += x, x => _locationManager.Value.DidVisit -= x)
                    .Select(x => new VisitedNotification());

            Failed =
                Observable.FromEvent<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(
                        x => _locationManager.Value.Failed += x, x => _locationManager.Value.Failed -= x)
                    .Select(x => new ErrorNotification());

            LocationUpdatesPaused =
                Observable
                    .FromEvent<EventHandler, Unit>(
                        x => _locationManager.Value.LocationUpdatesPaused += x,
                        x => _locationManager.Value.LocationUpdatesPaused -= x);

            LocationUpdatesResumed =
                Observable
                    .FromEvent<EventHandler, Unit>(
                        x => _locationManager.Value.LocationUpdatesResumed += x,
                        x => _locationManager.Value.LocationUpdatesResumed -= x);

            LocationsUpdated =
                Observable.FromEvent<EventHandler<CLLocationsUpdatedEventArgs>, CLLocationsUpdatedEventArgs>(
                        x => _locationManager.Value.LocationsUpdated += x,
                        x => _locationManager.Value.LocationsUpdated -= x)
                    .Select(x => new LocationsUpdatedNotification());

            MonitoringFailed =
                Observable.FromEvent<EventHandler<CLRegionErrorEventArgs>, CLRegionErrorEventArgs>(
                        x => _locationManager.Value.MonitoringFailed += x,
                        x => _locationManager.Value.MonitoringFailed -= x)
                    .Select(x => new RegionErrorNotification());

            RangingBeaconsDidFailForRegion =
                Observable.FromEvent<EventHandler<CLRegionBeaconsFailedEventArgs>, CLRegionBeaconsFailedEventArgs>(
                        x => _locationManager.Value.RangingBeaconsDidFailForRegion += x,
                        x => _locationManager.Value.RangingBeaconsDidFailForRegion -= x)
                    .Select(x => new RegionBeaconsFailedNotification());

            RegionEntered =
                Observable.FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        x => _locationManager.Value.RegionEntered += x,
                        x => _locationManager.Value.RegionEntered -= x)
                    .Select(x => new RegionChangedNotification());

            RegionExited =
                Observable.FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        x => _locationManager.Value.RegionLeft += x,
                        x => _locationManager.Value.RegionLeft -= x)
                    .Select(x => new RegionChangedNotification());

            UpdatedHeading =
                Observable.FromEvent<EventHandler<CLHeadingUpdatedEventArgs>, CLHeadingUpdatedEventArgs>(
                        x => _locationManager.Value.UpdatedHeading += x,
                        x => _locationManager.Value.UpdatedHeading -= x)
                    .Select(x => new HeadingUpdatedNotification());

            UpdatedLocation =
                Observable.FromEvent<EventHandler<CLLocationUpdatedEventArgs>, CLLocationUpdatedEventArgs>(
                        x => _locationManager.Value.UpdatedLocation += x,
                        x => _locationManager.Value.UpdatedLocation -= x)
                    .Select(x => new LocationUpdatedNotification());
        }

        /// <inheritdoc />
        public IObservable<AuthorizationChangedNotification> AuthorizationChanged { get; }

        /// <inheritdoc />
        public IObservable<ErrorNotification> DeferredUpdatesFinished { get; }

        /// <inheritdoc />
        public IObservable<RegionChangedNotification> DidDetermineState { get; }

        /// <inheritdoc />
        public IObservable<RegionBeaconsConstraintFailedNotification> DidFailRangingBeacons { get; }

        /// <inheritdoc />
        public IObservable<RegionBeaconRangedNotification> DidRangeBeacons { get; }

        /// <inheritdoc />
        public IObservable<RegionBeaconsConstraintRangedNotification> DidRangeBeaconsSatisfyingConstraint { get; }

        /// <inheritdoc />
        public IObservable<RegionChangedNotification> DidStartMonitoringForRegion { get; }

        /// <inheritdoc />
        public IObservable<VisitedNotification> DidVisit { get; }

        /// <inheritdoc />
        public IObservable<ErrorNotification> Failed { get; }

        /// <inheritdoc />
        public IObservable<Unit> LocationUpdatesPaused { get; }

        /// <inheritdoc />
        public IObservable<Unit> LocationUpdatesResumed { get; }

        /// <inheritdoc />
        public IObservable<LocationsUpdatedNotification> LocationsUpdated { get; }

        /// <inheritdoc />
        public IObservable<RegionErrorNotification> MonitoringFailed { get; }

        /// <inheritdoc />
        public IObservable<RegionBeaconsFailedNotification> RangingBeaconsDidFailForRegion { get; }

        /// <inheritdoc />
        public IObservable<RegionChangedNotification> RegionEntered { get; }

        /// <inheritdoc />
        public IObservable<RegionChangedNotification> RegionExited { get; }

        /// <inheritdoc />
        public IObservable<HeadingUpdatedNotification> UpdatedHeading { get; }

        /// <inheritdoc />
        public IObservable<LocationUpdatedNotification> UpdatedLocation { get; }
    }
}