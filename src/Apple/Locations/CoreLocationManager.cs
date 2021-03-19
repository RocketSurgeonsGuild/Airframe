using System;
using System.Reactive;
using System.Reactive.Linq;
using CoreLocation;
using Foundation;
using Rocket.Surgery.Airframe;

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
                    .Select(LocationEventExtensions.ToNotification);

            DeferredUpdatesFinished =
                Observable
                    .FromEvent<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(
                    x => _locationManager.Value.DeferredUpdatesFinished += x,
                    x => _locationManager.Value.DeferredUpdatesFinished -= x)
                    .Select(LocationEventExtensions.ToNotification);

            DidDetermineState =
                Observable
                    .FromEvent<EventHandler<CLRegionStateDeterminedEventArgs>, CLRegionStateDeterminedEventArgs>(
                        x => _locationManager.Value.DidDetermineState += x,
                        x => _locationManager.Value.DidDetermineState -= x)
                    .Select(LocationEventExtensions.ToNotification);

            DidFailRangingBeacons =
                Observable
                    .FromEvent<EventHandler<CLRegionBeaconsConstraintFailedEventArgs>,
                        CLRegionBeaconsConstraintFailedEventArgs>(
                        x => _locationManager.Value.DidFailRangingBeacons += x,
                        x => _locationManager.Value.DidFailRangingBeacons -= x)
                    .Select(LocationEventExtensions.ToNotification);

            DidRangeBeaconsSatisfyingConstraint =
                Observable
                    .FromEvent<EventHandler<CLRegionBeaconsConstraintRangedEventArgs>, CLRegionBeaconsConstraintRangedEventArgs>(
                        x => _locationManager.Value.DidRangeBeaconsSatisfyingConstraint += x,
                        x => _locationManager.Value.DidRangeBeaconsSatisfyingConstraint -= x)
                    .Select(LocationEventExtensions.ToNotification);

            DidStartMonitoringForRegion =
                Observable
                    .FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        x => _locationManager.Value.DidStartMonitoringForRegion += x,
                        x => _locationManager.Value.DidStartMonitoringForRegion -= x)
                    .Select(LocationEventExtensions.ToNotification);

            DidVisit =
                Observable
                    .FromEvent<EventHandler<CLVisitedEventArgs>, CLVisitedEventArgs>(
                        x => _locationManager.Value.DidVisit += x, x => _locationManager.Value.DidVisit -= x)
                    .Select(LocationEventExtensions.ToNotification);

            Failed =
                Observable
                    .FromEvent<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(
                        x => _locationManager.Value.Failed += x, x => _locationManager.Value.Failed -= x)
                    .Select(LocationEventExtensions.ToNotification);

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
                Observable
                    .FromEvent<EventHandler<CLLocationsUpdatedEventArgs>, CLLocationsUpdatedEventArgs>(
                        x => _locationManager.Value.LocationsUpdated += x,
                        x => _locationManager.Value.LocationsUpdated -= x)
                    .Select(LocationEventExtensions.ToNotification);

            MonitoringFailed =
                Observable
                    .FromEvent<EventHandler<CLRegionErrorEventArgs>, CLRegionErrorEventArgs>(
                        x => _locationManager.Value.MonitoringFailed += x,
                        x => _locationManager.Value.MonitoringFailed -= x)
                    .Select(LocationEventExtensions.ToNotification);

            RegionEntered =
                Observable
                    .FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        x => _locationManager.Value.RegionEntered += x,
                        x => _locationManager.Value.RegionEntered -= x)
                    .Select(LocationEventExtensions.ToNotification);

            RegionExited =
                Observable.FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        x => _locationManager.Value.RegionLeft += x,
                        x => _locationManager.Value.RegionLeft -= x)
                    .Select(LocationEventExtensions.ToNotification);

            UpdatedHeading =
                Observable
                    .FromEvent<EventHandler<CLHeadingUpdatedEventArgs>, CLHeadingUpdatedEventArgs>(
                        x => _locationManager.Value.UpdatedHeading += x,
                        x => _locationManager.Value.UpdatedHeading -= x)
                    .Select(LocationEventExtensions.ToNotification);

            UpdatedLocation =
                Observable
                    .FromEvent<EventHandler<CLLocationUpdatedEventArgs>, CLLocationUpdatedEventArgs>(
                        x => _locationManager.Value.UpdatedLocation += x,
                        x => _locationManager.Value.UpdatedLocation -= x)
                    .Select(LocationEventExtensions.ToNotification);
        }

        /// <inheritdoc />
        public IObservable<AuthorizationChangedEvent> AuthorizationChanged { get; }

        /// <inheritdoc />
        public IObservable<ErrorEvent> DeferredUpdatesFinished { get; }

        /// <inheritdoc />
        public IObservable<RegionChangedEvent> DidDetermineState { get; }

        /// <inheritdoc />
        public IObservable<RegionBeaconsConstraintFailedEvent> DidFailRangingBeacons { get; }

        /// <inheritdoc />
        public IObservable<RegionBeaconsConstraintRangedEvent> DidRangeBeaconsSatisfyingConstraint { get; }

        /// <inheritdoc />
        public IObservable<RegionChangedEvent> DidStartMonitoringForRegion { get; }

        /// <inheritdoc />
        public IObservable<VisitedEvent> DidVisit { get; }

        /// <inheritdoc />
        public IObservable<ErrorEvent> Failed { get; }

        /// <inheritdoc />
        public IObservable<Unit> LocationUpdatesPaused { get; }

        /// <inheritdoc />
        public IObservable<Unit> LocationUpdatesResumed { get; }

        /// <inheritdoc />
        public IObservable<LocationsUpdatedEvent> LocationsUpdated { get; }

        /// <inheritdoc />
        public IObservable<RegionErrorEvent> MonitoringFailed { get; }

        /// <inheritdoc />
        public IObservable<RegionChangedEvent> RegionEntered { get; }

        /// <inheritdoc />
        public IObservable<RegionChangedEvent> RegionExited { get; }

        /// <inheritdoc />
        public IObservable<HeadingUpdatedEvent> UpdatedHeading { get; }

        /// <inheritdoc />
        public IObservable<LocationUpdatedEvent> UpdatedLocation { get; }
    }
}
