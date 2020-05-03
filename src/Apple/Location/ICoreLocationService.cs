using System;
using System.Reactive;
using CoreLocation;
using Foundation;
using Rocket.Surgery.Airframe.Apple.Notifications;

namespace Rocket.Surgery.Airframe.Apple
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
        IObservable<RegionErrorNotification> MonitoringFailed { get; }

        /// <summary>
        /// Gets or sets an observable sequence of Visited events.
        /// </summary>
        IObservable<VisitedNotification> Visited { get; }

        /// <summary>
        /// Gets or sets an observable sequence of deferred update completions.
        /// </summary>
        IObservable<ErrorNotification> DeferredUpdatesFinished { get; }

        /// <summary>
        /// Gets or sets an observable sequence of notifications when region state is determined.
        /// </summary>
        IObservable<RegionChangedNotification> RegionStateDetermined { get; }

        /// <summary>
        /// Gets or sets an observable sequence of regions being monitored.
        /// </summary>
        IObservable<RegionChangedNotification> MonitoringRegion { get; }

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
}