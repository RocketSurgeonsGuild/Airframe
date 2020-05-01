using System;
using System.Reactive;
using System.Reactive.Linq;
using CoreLocation;
using Foundation;
using Rocket.Surgery.Airframe.iOS.Notifications;

namespace Rocket.Surgery.Airframe.iOS
{
    public interface ICoreLocationService
    {
        /// <summary>
        /// Gets or sets the location update notifications.
        /// </summary>
        IObservable<LocationUpdatedNotification> LocationUpdated { get; }

        /// <summary>
        /// Gets or sets whether the monitoring failed.
        /// </summary>
        IObservable<CLRegionErrorEventArgs> MonitoringFailed { get; }

        /// <summary>
        /// Gets or sets an observable sequence of Visited events.
        /// </summary>
        IObservable<CLVisitedEventArgs> Visited { get; }

        /// <summary>
        /// Gets or sets an observable sequence of deferred update completions.
        /// </summary>
        IObservable<NSErrorEventArgs> DeferredUpdatesFinished { get; }

        /// <summary>
        /// Gets or sets an observable sequence of notifications when region state is determined.
        /// </summary>
        IObservable<RegionStateDeterminedNotification> RegionStateDetermined { get; }

        /// <summary>
        /// Gets or sets an observable sequence of regions being monitored.
        /// </summary>
        IObservable<RegionNotification> MonitoringRegion { get; }

        /// <summary>
        /// Gets or sets an observable sequence notifying when location updates resume.
        /// </summary>
        public IObservable<Unit> LocationUpdatesResumed { get; }

        /// <summary>
        /// Gets or sets an observable sequence notifying when location updates paused.
        /// </summary>
        public IObservable<Unit> LocationUpdatesPaused { get; }

        /// <summary>
        /// Gets or sets an observable sequence notifying when location updates resume.
        /// </summary>
        IObservable<LocationsUpdatedNotification> LocationsUpdated { get; }

        /// <summary>
        /// Gets or sets an observable sequence notifying that authorization change.
        /// </summary>
        IObservable<AuthorizationChangedNotification> AuthorizationChanged { get; }
    }

    public class CoreLocationService : ICoreLocationService
    {
        private readonly ILocationManager _locationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreLocationService"/> class.
        /// </summary>
        /// <param name="locationManager">The underlying platform location services.</param>
        public CoreLocationService(ILocationManager locationManager)
        {
            _locationManager = locationManager;

            AuthorizationChanged =
                Observable
                    .FromEvent<EventHandler<CLAuthorizationChangedEventArgs>, CLAuthorizationChangedEventArgs>(
                        eventHandler =>
                        {
                            void Handler(object sender, CLAuthorizationChangedEventArgs args) => eventHandler(args);
                            return Handler;
                        },
                        x => _locationManager.AuthorizationChanged += x,
                        x => _locationManager.AuthorizationChanged -= x)
                    .Select(args => args.ToNotification());

            LocationsUpdated =
                Observable
                    .FromEvent<EventHandler<CLLocationsUpdatedEventArgs>, CLLocationsUpdatedEventArgs>(
                        x => _locationManager.LocationsUpdated += x,
                        x => _locationManager.LocationsUpdated -= x)
                    .Select(args => args.ToNotification());

            LocationUpdated =
                Observable
                    .FromEvent<EventHandler<CLLocationUpdatedEventArgs>, CLLocationUpdatedEventArgs>(
                        x => _locationManager.UpdatedLocation += x,
                        x => _locationManager.UpdatedLocation -= x)
                    .Select(args => args.ToNotification());

            LocationUpdatesPaused =
                Observable
                    .FromEventPattern(
                        x => _locationManager.LocationUpdatesPaused += x,
                        x => _locationManager.LocationUpdatesPaused -= x)
                    .Select(_ => Unit.Default);

            LocationUpdatesResumed =
                Observable
                    .FromEventPattern(
                        x => _locationManager.LocationUpdatesResumed += x,
                        x => _locationManager.LocationUpdatesResumed -= x)
                    .Select(_ => Unit.Default);

            MonitoringRegion =
                Observable
                    .FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        x => _locationManager.DidStartMonitoringForRegion += x,
                        x => _locationManager.DidStartMonitoringForRegion -= x)
                    .Select(args => args.ToNotification());

            RegionStateDetermined =
                Observable
                    .FromEvent<EventHandler<CLRegionStateDeterminedEventArgs>, CLRegionStateDeterminedEventArgs>(
                        x => _locationManager.DidDetermineState += x,
                        x => _locationManager.DidDetermineState -= x)
                    .Select(x => x.ToNotification());

            DeferredUpdatesFinished =
                Observable
                    .FromEvent<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(
                        x => _locationManager.DeferredUpdatesFinished += x,
                        x => _locationManager.DeferredUpdatesFinished -= x);

            Visited =
                Observable
                    .FromEvent<EventHandler<CLVisitedEventArgs>, CLVisitedEventArgs>(
                        x => _locationManager.DidVisit += x,
                        x => _locationManager.DidVisit -= x);

            MonitoringFailed =
                Observable
                    .FromEvent<EventHandler<CLRegionErrorEventArgs>, CLRegionErrorEventArgs>(
                        x => _locationManager.MonitoringFailed += x,
                        x => _locationManager.MonitoringFailed -= x);
        }

        /// <inheritdoc/>
        public IObservable<LocationUpdatedNotification> LocationUpdated { get; }

        /// <inheritdoc/>
        public IObservable<CLRegionErrorEventArgs> MonitoringFailed { get; }

        /// <inheritdoc/>
        public IObservable<CLVisitedEventArgs> Visited { get; set; }

        /// <inheritdoc/>
        public IObservable<NSErrorEventArgs> DeferredUpdatesFinished { get; }

        /// <inheritdoc/>
        public IObservable<RegionStateDeterminedNotification> RegionStateDetermined { get; }

        /// <inheritdoc/>
        public IObservable<RegionNotification> MonitoringRegion { get; }

        /// <inheritdoc/>
        public IObservable<Unit> LocationUpdatesResumed { get; }

        /// <inheritdoc/>
        public IObservable<Unit> LocationUpdatesPaused { get; }

        /// <inheritdoc/>
        public IObservable<LocationsUpdatedNotification> LocationsUpdated { get; }

        /// <inheritdoc/>
        public IObservable<AuthorizationChangedNotification> AuthorizationChanged { get; }
    }
}