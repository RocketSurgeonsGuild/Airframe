using System;
using System.Reactive;
using System.Reactive.Linq;
using CoreLocation;
using Foundation;

namespace Rocket.Surgery.Airframe.iOS
{
    public interface ICoreLocationService
    {
        /// <summary>
        /// Gets or sets the location update notifications.
        /// </summary>
        IObservable<CLLocationUpdatedEventArgs> LocationUpdated { get; set; }

        /// <summary>
        /// Gets or sets whether the monitoring failed.
        /// </summary>
        IObservable<CLRegionErrorEventArgs> MonitoringFailed { get; set; }

        /// <summary>
        /// Gets or sets an observable sequence of Visited events.
        /// </summary>
        IObservable<CLVisitedEventArgs> Visited { get; set; }
        IObservable<NSErrorEventArgs> DeferredUpdatesFinished { get; set; }
        IObservable<CLRegionStateDeterminedEventArgs> RegionStateDetermined { get; set; }
        IObservable<CLRegionEventArgs> MonitoringRegion { get; set; }
        IObservable<EventPattern<object>> LocationUpdatesResumed { get; set; }
        IObservable<EventPattern<object>> LocationUpdatesPaused { get; set; }
        IObservable<CLLocationsUpdatedEventArgs> LocationsUpdated { get; set; }
        IObservable<CLAuthorizationChangedEventArgs> AuthorizationChanged { get; set; }
    }

    public class CoreLocationService : ICoreLocationService
    {
        private readonly Lazy<CLLocationManager> _locationManager = new Lazy<CLLocationManager>();

        public CoreLocationService()
        {
            AuthorizationChanged =
                Observable
                    .FromEvent<EventHandler<CLAuthorizationChangedEventArgs>, CLAuthorizationChangedEventArgs>(
                        eventHandler =>
                        {
                            void Handler(object sender, CLAuthorizationChangedEventArgs args) => eventHandler(args);
                            return Handler;
                        },
                        x => _locationManager.Value.AuthorizationChanged += x,
                        x => _locationManager.Value.AuthorizationChanged -= x);

            LocationsUpdated =
                Observable
                    .FromEvent<EventHandler<CLLocationsUpdatedEventArgs>, CLLocationsUpdatedEventArgs>(
                        x => _locationManager.Value.LocationsUpdated += x,
                        x => _locationManager.Value.LocationsUpdated -= x);


            LocationUpdated =
                Observable
                    .FromEvent<EventHandler<CLLocationUpdatedEventArgs>, CLLocationUpdatedEventArgs>(
                        x => _locationManager.Value.UpdatedLocation += x,
                        x => _locationManager.Value.UpdatedLocation -= x);

            LocationUpdatesPaused =
                Observable
                    .FromEventPattern(
                        x => _locationManager.Value.LocationUpdatesPaused += x,
                        x => _locationManager.Value.LocationUpdatesPaused -= x);

            LocationUpdatesResumed =
                Observable
                    .FromEventPattern(
                        x => _locationManager.Value.LocationUpdatesResumed += x,
                        x => _locationManager.Value.LocationUpdatesResumed -= x);

            MonitoringRegion =
                Observable
                    .FromEvent<EventHandler<CLRegionEventArgs>, CLRegionEventArgs>(
                        x => _locationManager.Value.DidStartMonitoringForRegion += x,
                        x => _locationManager.Value.DidStartMonitoringForRegion -= x);

            RegionStateDetermined =
                Observable
                    .FromEvent<EventHandler<CLRegionStateDeterminedEventArgs>, CLRegionStateDeterminedEventArgs>(
                        x => _locationManager.Value.DidDetermineState += x,
                        x => _locationManager.Value.DidDetermineState -= x);

            DeferredUpdatesFinished =
                Observable
                    .FromEvent<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(
                        x => _locationManager.Value.DeferredUpdatesFinished += x,
                        x => _locationManager.Value.DeferredUpdatesFinished -= x);

            Visited =
                Observable
                    .FromEvent<EventHandler<CLVisitedEventArgs>, CLVisitedEventArgs>(
                        x => _locationManager.Value.DidVisit += x,
                        x => _locationManager.Value.DidVisit -= x);

            MonitoringFailed =
                Observable
                    .FromEvent<EventHandler<CLRegionErrorEventArgs>, CLRegionErrorEventArgs>(
                        x => _locationManager.Value.MonitoringFailed += x,
                        x => _locationManager.Value.MonitoringFailed -= x);
        }

        public IObservable<CLLocationUpdatedEventArgs> LocationUpdated { get; set; }

        public IObservable<CLRegionErrorEventArgs> MonitoringFailed { get; set; }

        public IObservable<CLVisitedEventArgs> Visited { get; set; }

        public IObservable<NSErrorEventArgs> DeferredUpdatesFinished { get; set; }

        public IObservable<CLRegionStateDeterminedEventArgs> RegionStateDetermined { get; set; }

        public IObservable<CLRegionEventArgs> MonitoringRegion { get; set; }

        public IObservable<EventPattern<object>> LocationUpdatesResumed { get; set; }

        public IObservable<EventPattern<object>> LocationUpdatesPaused { get; set; }

        public IObservable<CLLocationsUpdatedEventArgs> LocationsUpdated { get; set; }

        public IObservable<CLAuthorizationChangedEventArgs> AuthorizationChanged { get; set; }
    }
}