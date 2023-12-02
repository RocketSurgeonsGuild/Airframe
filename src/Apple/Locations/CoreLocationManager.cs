using System;
using System.Reactive;
using System.Reactive.Linq;
using CoreLocation;
using Foundation;

namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Implementation of <see cref="ICoreLocationService"/> on top of <see cref="CLLocationManager" />.
    /// </summary>
    public class CoreLocationManager : ICoreLocationService
    {
        private readonly Lazy<CLLocationManager> _locationManager = new(() => new CLLocationManager());

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreLocationManager"/> class.
        /// </summary>
        public CoreLocationManager()
        {
            AuthorizationChanged =
                Observable
                    .FromEvent<EventHandler<CLAuthorizationChangedEventArgs>, CLAuthorizationChangedEventArgs>(
                        handler => _locationManager.Value.AuthorizationChanged += handler,
                        handler => _locationManager.Value.AuthorizationChanged -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            DeferredUpdatesFinished =
                Observable
                    .FromEvent<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(
                    handler => _locationManager.Value.DeferredUpdatesFinished += handler,
                    handler => _locationManager.Value.DeferredUpdatesFinished -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            DeterminedState =
                Observable
                    .FromEvent<EventHandler<CLRegionStateDeterminedEventArgs>, CLRegionStateDeterminedEventArgs>(
                        handler => _locationManager.Value.DidDetermineState += handler,
                        handler => _locationManager.Value.DidDetermineState -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            FailedRangingBeacons =
                Observable
                    .FromEvent<EventHandler<CLRegionBeaconsConstraintFailedEventArgs>,
                        CLRegionBeaconsConstraintFailedEventArgs>(
                        handler => _locationManager.Value.DidFailRangingBeacons += handler,
                        handler => _locationManager.Value.DidFailRangingBeacons -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            RangedBeaconsSatisfyingConstraint =
                Observable
                    .FromEvent<EventHandler<CLRegionBeaconsConstraintRangedEventArgs>, CLRegionBeaconsConstraintRangedEventArgs>(
                        handler => _locationManager.Value.DidRangeBeaconsSatisfyingConstraint += handler,
                        handler => _locationManager.Value.DidRangeBeaconsSatisfyingConstraint -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            StartedMonitoringForRegion =
                Observable
                    .FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        handler => _locationManager.Value.DidStartMonitoringForRegion += handler,
                        handler => _locationManager.Value.DidStartMonitoringForRegion -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            Visited =
                Observable
                    .FromEvent<EventHandler<CLVisitedEventArgs>, CLVisitedEventArgs>(
                        handler => _locationManager.Value.DidVisit += handler,
                        handler => _locationManager.Value.DidVisit -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            Failed =
                Observable
                    .FromEvent<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(
                        handler => _locationManager.Value.Failed += handler,
                        handler => _locationManager.Value.Failed -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            LocationUpdatesPaused =
                Observable
                    .FromEvent<EventHandler, Unit>(
                        handler => _locationManager.Value.LocationUpdatesPaused += handler,
                        handler => _locationManager.Value.LocationUpdatesPaused -= handler);

            LocationUpdatesResumed =
                Observable
                    .FromEvent<EventHandler, Unit>(
                        handler => _locationManager.Value.LocationUpdatesResumed += handler,
                        handler => _locationManager.Value.LocationUpdatesResumed -= handler);

            LocationsUpdated =
                Observable
                    .FromEvent<EventHandler<CLLocationsUpdatedEventArgs>, CLLocationsUpdatedEventArgs>(
                        handler => _locationManager.Value.LocationsUpdated += handler,
                        handler => _locationManager.Value.LocationsUpdated -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            MonitoringFailed =
                Observable
                    .FromEvent<EventHandler<CLRegionErrorEventArgs>, CLRegionErrorEventArgs>(
                        handler => _locationManager.Value.MonitoringFailed += handler,
                        handler => _locationManager.Value.MonitoringFailed -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            RegionEntered =
                Observable
                    .FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        handler => _locationManager.Value.RegionEntered += handler,
                        handler => _locationManager.Value.RegionEntered -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            RegionExited =
                Observable.FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        handler => _locationManager.Value.RegionLeft += handler,
                        handler => _locationManager.Value.RegionLeft -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            UpdatedHeading =
                Observable
                    .FromEvent<EventHandler<CLHeadingUpdatedEventArgs>, CLHeadingUpdatedEventArgs>(
                        handler => _locationManager.Value.UpdatedHeading += handler,
                        handler => _locationManager.Value.UpdatedHeading -= handler)
                    .Select(LocationEventExtensions.ToNotification);

            UpdatedLocation =
                Observable
                    .FromEvent<EventHandler<CLLocationUpdatedEventArgs>, CLLocationUpdatedEventArgs>(
                        handler => _locationManager.Value.UpdatedLocation += handler,
                        handler => _locationManager.Value.UpdatedLocation -= handler)
                    .Select(LocationEventExtensions.ToNotification);
        }

        /// <inheritdoc />
        public IObservable<AuthorizationChangedEvent> AuthorizationChanged { get; }

        /// <inheritdoc />
        public IObservable<ErrorEvent> DeferredUpdatesFinished { get; }

        /// <inheritdoc />
        public IObservable<RegionChangedEvent> DeterminedState { get; }

        /// <inheritdoc />
        public IObservable<RegionBeaconsConstraintFailedEvent> FailedRangingBeacons { get; }

        /// <inheritdoc />
        public IObservable<RegionBeaconsConstraintRangedEvent> RangedBeaconsSatisfyingConstraint { get; }

        /// <inheritdoc />
        public IObservable<RegionChangedEvent> StartedMonitoringForRegion { get; }

        /// <inheritdoc />
        public IObservable<VisitedEvent> Visited { get; }

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
