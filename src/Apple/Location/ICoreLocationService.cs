using System;
using System.Reactive;

namespace Rocket.Surgery.Airframe.Apple
{
    public interface ICoreLocationService
    {
        /// <summary>
        /// Gets the location update notifications.
        /// </summary>
        IObservable<LocationUpdatedNotification> LocationUpdated { get; }

        /// <summary>
        /// Gets whether the monitoring failed.
        /// </summary>
        IObservable<RegionErrorNotification> MonitoringFailed { get; }

        /// <summary>
        /// Gets an observable sequence of Visited events.
        /// </summary>
        IObservable<VisitedNotification> Visited { get; }

        /// <summary>
        /// Gets an observable sequence of deferred update completions.
        /// </summary>
        IObservable<ErrorNotification> DeferredUpdatesFinished { get; }

        /// <summary>
        /// Gets an observable sequence of notifications when region state is determined.
        /// </summary>
        IObservable<RegionChangedNotification> RegionStateDetermined { get; }

        /// <summary>
        /// Gets an observable sequence of regions being monitored.
        /// </summary>
        IObservable<RegionChangedNotification> MonitoringRegion { get; }

        /// <summary>
        /// Gets an observable sequence notifying when location updates resume.
        /// </summary>
        public IObservable<Unit> LocationUpdatesResumed { get; }

        /// <summary>
        /// Gets an observable sequence notifying when location updates paused.
        /// </summary>
        public IObservable<Unit> LocationUpdatesPaused { get; }

        /// <summary>
        /// Gets an observable sequence notifying when location updates resume.
        /// </summary>
        IObservable<LocationsUpdatedNotification> LocationsUpdated { get; }

        /// <summary>
        /// Gets an observable sequence notifying that authorization change.
        /// </summary>
        IObservable<AuthorizationChangedNotification> AuthorizationChanged { get; }
    }
}