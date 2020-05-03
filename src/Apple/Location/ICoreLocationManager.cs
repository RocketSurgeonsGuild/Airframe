using System;
using System.Reactive;
using Rocket.Surgery.Airframe.Apple.Notifications;

namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Interface that represents a CLLocationManager.
    /// </summary>
    public interface ICoreLocationManager // TODO: Depends on aggregating the platform events
    {
        /// <summary>
        /// Gets an observable sequence notifying that authorization change.
        /// </summary>
        IObservable<AuthorizationChangedNotification> AuthorizationChanged { get; }

        /// <summary>
        /// Gets an observable sequence of deferred update completions.
        /// </summary>
        IObservable<ErrorNotification> DeferredUpdatesFinished { get; }

        /// <summary>
        /// Gets an observable sequence of notifications when region state is determined.
        /// </summary>
        IObservable<RegionChangedNotification> DidDetermineState { get; }

        /// <summary>
        /// Gets an observable sequence of region beacon failure notifications.
        /// </summary>
        IObservable<RegionBeaconsConstraintFailedNotification> DidFailRangingBeacons { get; }

        /// <summary>
        /// Gets an observable sequence of ranged beacon notifications.
        /// </summary>
        IObservable<RegionBeaconRangedNotification> DidRangeBeacons { get; }

        /// <summary>
        /// Gets an observable sequence of beacon range constraint notifications.
        /// </summary>
        IObservable<RegionBeaconsConstraintRangedNotification> DidRangeBeaconsSatisfyingConstraint { get; }

        /// <summary>
        /// Gets an observable sequence of regions when the are started being monitored.
        /// </summary>
        IObservable<RegionChangedNotification> DidStartMonitoringForRegion { get; }

        /// <summary>
        /// Gets an observable sequence of regions visited.
        /// </summary>
        IObservable<VisitedNotification> DidVisit { get; }

        /// <summary>
        /// Gets an observable sequence of failure notifications.
        /// </summary>
        IObservable<ErrorNotification> Failed { get; }

        /// <summary>
        /// Gets an observable sequence notifying when location updates paused.
        /// </summary>
        IObservable<Unit> LocationUpdatesPaused { get; }

        /// <summary>
        /// Gets an observable sequence notifying when location updates resume.
        /// </summary>
        IObservable<Unit> LocationUpdatesResumed { get; }

        /// <summary>
        /// Gets an observable sequence notifying when location updates resume.
        /// </summary>
        IObservable<LocationsUpdatedNotification> LocationsUpdated { get; }

        /// <summary>
        /// Gets whether the monitoring failed.
        /// </summary>
        IObservable<RegionErrorNotification> MonitoringFailed { get; }

        /// <summary>
        /// Gets an observable sequence of failure notifications.
        /// </summary>
        IObservable<RegionBeaconsFailedNotification> RangingBeaconsDidFailForRegion { get; }

        /// <summary>
        /// Gets an observable sequence notifying when a region has been entered.
        /// </summary>
        IObservable<RegionChangedNotification> RegionEntered { get; }

        /// <summary>
        /// Gets an observable sequence notifying when a region has been exited.
        /// </summary>
        IObservable<RegionChangedNotification> RegionExited { get; }

        /// <summary>
        /// Gets an observable sequence of heading update notifications.
        /// </summary>
        IObservable<HeadingUpdatedNotification> UpdatedHeading { get; }

        /// <summary>
        /// Gets the location update notifications.
        /// </summary>
        IObservable<LocationUpdatedNotification> UpdatedLocation { get; }
    }
}