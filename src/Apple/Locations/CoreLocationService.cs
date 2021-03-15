using System;
using System.Reactive;
using System.Reactive.Linq;
using CoreLocation;
using Foundation;

namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Represents an apple <see cref="ICoreLocationService"/>.
    /// </summary>
    public class CoreLocationService : ICoreLocationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreLocationService"/> class.
        /// </summary>
        public CoreLocationService()
            : this(new CLLocationManagerDecorator(new CLLocationManager()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreLocationService"/> class.
        /// </summary>
        /// <param name="locationManager">The underlying platform location services.</param>
        public CoreLocationService(ICLLocationManager locationManager)
        {
            // TODO: Allow certain observables to connect and disconnect from events depending on whether location services are changed on/off, and other criteria.
            AuthorizationChanged =
                Observable
                    .FromEvent<EventHandler<CLAuthorizationChangedEventArgs>, CLAuthorizationChangedEventArgs>(
                        handler =>
                        {
                            void Handler(object sender, CLAuthorizationChangedEventArgs args) => handler(args);
                            return Handler;
                        },
                        x => locationManager.AuthorizationChanged += x,
                        x => locationManager.AuthorizationChanged -= x)
                    .Select(LocationEventExtensions.ToNotification);

            LocationsUpdated =
                Observable
                    .FromEvent<EventHandler<CLLocationsUpdatedEventArgs>, CLLocationsUpdatedEventArgs>(
                        handler =>
                        {
                            void Handler(object sender, CLLocationsUpdatedEventArgs args) => handler(args);
                            return Handler;
                        },
                        x => locationManager.LocationsUpdated += x,
                        x => locationManager.LocationsUpdated -= x)
                    .Select(LocationEventExtensions.ToNotification);

            LocationUpdated =
                Observable
                    .FromEvent<EventHandler<CLLocationUpdatedEventArgs>, CLLocationUpdatedEventArgs>(
                        handler =>
                        {
                            void Handler(object sender, CLLocationUpdatedEventArgs args) => handler(args);
                            return Handler;
                        },
                        x => locationManager.UpdatedLocation += x,
                        x => locationManager.UpdatedLocation -= x)
                    .Select(LocationEventExtensions.ToNotification);

            LocationUpdatesPaused =
                Observable
                    .FromEvent<EventHandler, EventArgs>(
                        handler =>
                        {
                            void Handler(object sender, EventArgs args) => handler(args);
                            return Handler;
                        },
                        x => locationManager.LocationUpdatesPaused += x,
                        x => locationManager.LocationUpdatesPaused -= x)
                    .Select(LocationEventExtensions.ToNotification);

            LocationUpdatesResumed =
                Observable
                    .FromEvent<EventHandler, EventArgs>(
                        handler =>
                        {
                            void Handler(object sender, EventArgs args) => handler(args);
                            return Handler;
                        },
                        x => locationManager.LocationUpdatesResumed += x,
                        x => locationManager.LocationUpdatesResumed -= x)
                    .Select(LocationEventExtensions.ToNotification);

            MonitoringRegion =
                Observable
                    .FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        handler =>
                        {
                            void Handler(object sender, CLRegionEventArgs args) => handler(args);
                            return Handler;
                        },
                        x => locationManager.DidStartMonitoringForRegion += x,
                        x => locationManager.DidStartMonitoringForRegion -= x)
                    .Select(LocationEventExtensions.ToNotification);

            RegionStateDetermined =
                Observable
                    .FromEvent<EventHandler<CLRegionStateDeterminedEventArgs>, CLRegionStateDeterminedEventArgs>(
                        handler =>
                        {
                            void Handler(object sender, CLRegionStateDeterminedEventArgs args) => handler(args);
                            return Handler;
                        },
                        x => locationManager.DidDetermineState += x,
                        x => locationManager.DidDetermineState -= x)
                    .Select(LocationEventExtensions.ToNotification);

            DeferredUpdatesFinished =
                Observable
                    .FromEvent<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(
                        handler =>
                        {
                            void Handler(object sender, NSErrorEventArgs args) => handler(args);
                            return Handler;
                        },
                        x => locationManager.DeferredUpdatesFinished += x,
                        x => locationManager.DeferredUpdatesFinished -= x)
                    .Select(LocationEventExtensions.ToNotification);

            Visited =
                Observable
                    .FromEvent<EventHandler<CLVisitedEventArgs>, CLVisitedEventArgs>(
                        handler =>
                        {
                            void Handler(object sender, CLVisitedEventArgs args) => handler(args);
                            return Handler;
                        },
                        x => locationManager.DidVisit += x,
                        x => locationManager.DidVisit -= x)
                    .Select(LocationEventExtensions.ToNotification);

            MonitoringFailed =
                Observable
                    .FromEvent<EventHandler<CLRegionErrorEventArgs>, CLRegionErrorEventArgs>(
                        handler =>
                        {
                            void Handler(object sender, CLRegionErrorEventArgs args) => handler(args);
                            return Handler;
                        },
                        x => locationManager.MonitoringFailed += x,
                        x => locationManager.MonitoringFailed -= x)
                    .Select(LocationEventExtensions.ToNotification);
        }

        /// <inheritdoc/>
        public IObservable<LocationUpdatedEvent> LocationUpdated { get; }

        /// <inheritdoc/>
        public IObservable<RegionErrorEvent> MonitoringFailed { get; }

        /// <inheritdoc/>
        public IObservable<VisitedEvent> Visited { get; }

        /// <inheritdoc/>
        public IObservable<ErrorEvent> DeferredUpdatesFinished { get; }

        /// <inheritdoc/>
        public IObservable<RegionChangedEvent> RegionStateDetermined { get; }

        /// <inheritdoc/>
        public IObservable<RegionChangedEvent> MonitoringRegion { get; }

        /// <inheritdoc/>
        public IObservable<Unit> LocationUpdatesResumed { get; }

        /// <inheritdoc/>
        public IObservable<Unit> LocationUpdatesPaused { get; }

        /// <inheritdoc/>
        public IObservable<LocationsUpdatedEvent> LocationsUpdated { get; }

        /// <inheritdoc/>
        public IObservable<AuthorizationChangedEvent> AuthorizationChanged { get; }
    }
}