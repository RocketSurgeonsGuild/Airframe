using System;
using System.Reactive;
using System.Reactive.Linq;
using CoreLocation;
using Foundation;
using Rocket.Surgery.Airframe.Apple.Notifications;

namespace Rocket.Surgery.Airframe.Apple
{
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
                    .Select(NotificationExtensions.ToNotification);

            LocationsUpdated =
                Observable
                    .FromEvent<EventHandler<CLLocationsUpdatedEventArgs>, CLLocationsUpdatedEventArgs>(
                        x => _locationManager.LocationsUpdated += x,
                        x => _locationManager.LocationsUpdated -= x)
                    .Select(NotificationExtensions.ToNotification);

            LocationUpdated =
                Observable
                    .FromEvent<EventHandler<CLLocationUpdatedEventArgs>, CLLocationUpdatedEventArgs>(
                        x => _locationManager.UpdatedLocation += x,
                        x => _locationManager.UpdatedLocation -= x)
                    .Select(NotificationExtensions.ToNotification);

            LocationUpdatesPaused =
                Observable
                    .FromEventPattern(
                        x => _locationManager.LocationUpdatesPaused += x,
                        x => _locationManager.LocationUpdatesPaused -= x)
                    .Select(NotificationExtensions.ToNotification);

            LocationUpdatesResumed =
                Observable
                    .FromEventPattern(
                        x => _locationManager.LocationUpdatesResumed += x,
                        x => _locationManager.LocationUpdatesResumed -= x)
                    .Select(NotificationExtensions.ToNotification);

            MonitoringRegion =
                Observable
                    .FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        x => _locationManager.DidStartMonitoringForRegion += x,
                        x => _locationManager.DidStartMonitoringForRegion -= x)
                    .Select(NotificationExtensions.ToNotification);

            RegionStateDetermined =
                Observable
                    .FromEvent<EventHandler<CLRegionStateDeterminedEventArgs>, CLRegionStateDeterminedEventArgs>(
                        x => _locationManager.DidDetermineState += x,
                        x => _locationManager.DidDetermineState -= x)
                    .Select(NotificationExtensions.ToNotification);

            DeferredUpdatesFinished =
                Observable
                    .FromEvent<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(
                        x => _locationManager.DeferredUpdatesFinished += x,
                        x => _locationManager.DeferredUpdatesFinished -= x)
                    .Select(NotificationExtensions.ToNotification);

            Visited =
                Observable
                    .FromEvent<EventHandler<CLVisitedEventArgs>, CLVisitedEventArgs>(
                        x => _locationManager.DidVisit += x,
                        x => _locationManager.DidVisit -= x)
                    .Select(NotificationExtensions.ToNotification);

            MonitoringFailed =
                Observable
                    .FromEvent<EventHandler<CLRegionErrorEventArgs>, CLRegionErrorEventArgs>(
                        x => _locationManager.MonitoringFailed += x,
                        x => _locationManager.MonitoringFailed -= x)
                    .Select(NotificationExtensions.ToNotification);
        }

        /// <inheritdoc/>
        public IObservable<LocationUpdatedNotification> LocationUpdated { get; }

        /// <inheritdoc/>
        public IObservable<RegionErrorNotification> MonitoringFailed { get; }

        /// <inheritdoc/>
        public IObservable<VisitedNotification> Visited { get; }

        /// <inheritdoc/>
        public IObservable<ErrorNotification> DeferredUpdatesFinished { get; }

        /// <inheritdoc/>
        public IObservable<RegionChangedNotification> RegionStateDetermined { get; }

        /// <inheritdoc/>
        public IObservable<RegionChangedNotification> MonitoringRegion { get; }

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