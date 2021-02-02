using System;
using System.Reactive;
using Rocket.Surgery.Airframe.Locations.Events;

namespace Rocket.Surgery.Airframe.Locations
{
    /// <summary>
    /// Interface that represents a reactive CLLocationManager.
    /// </summary>
    public interface ICoreLocationManager
    {
        /// <summary>
        /// Gets an observable sequence notifying that authorization change.
        /// </summary>
        IObservable<AuthorizationChangedEvent> AuthorizationChanged { get; }

        /// <summary>
        /// Gets an observable sequence of deferred update completions.
        /// </summary>
        IObservable<ErrorEvent> DeferredUpdatesFinished { get; }

        /// <summary>
        /// Gets an observable sequence of notifications when region state is determined.
        /// </summary>
        IObservable<RegionChangedEvent> DidDetermineState { get; }

        /// <summary>
        /// Gets an observable sequence of region beacon failure notifications.
        /// </summary>
        IObservable<RegionBeaconsConstraintFailedEvent> DidFailRangingBeacons { get; }

        /// <summary>
        /// Gets an observable sequence of ranged beacon notifications.
        /// </summary>
        IObservable<RegionBeaconRangedEvent> DidRangeBeacons { get; }

        /// <summary>
        /// Gets an observable sequence of beacon range constraint notifications.
        /// </summary>
        IObservable<RegionBeaconsConstraintRangedNotification> DidRangeBeaconsSatisfyingConstraint { get; }

        /// <summary>
        /// Gets an observable sequence of regions when the are started being monitored.
        /// </summary>
        IObservable<RegionChangedEvent> DidStartMonitoringForRegion { get; }

        /// <summary>
        /// Gets an observable sequence of regions visited.
        /// </summary>
        IObservable<VisitedEvent> DidVisit { get; }

        /// <summary>
        /// Gets an observable sequence of failure notifications.
        /// </summary>
        IObservable<ErrorEvent> Failed { get; }

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
        IObservable<LocationsUpdatedEvent> LocationsUpdated { get; }

        /// <summary>
        /// Gets whether the monitoring failed.
        /// </summary>
        IObservable<RegionErrorEvent> MonitoringFailed { get; }

        /// <summary>
        /// Gets an observable sequence of failure notifications.
        /// </summary>
        IObservable<RegionBeaconsFailedEvent> RangingBeaconsDidFailForRegion { get; }

        /// <summary>
        /// Gets an observable sequence notifying when a region has been entered.
        /// </summary>
        IObservable<RegionChangedEvent> RegionEntered { get; }

        /// <summary>
        /// Gets an observable sequence notifying when a region has been exited.
        /// </summary>
        IObservable<RegionChangedEvent> RegionExited { get; }

        /// <summary>
        /// Gets an observable sequence of heading update notifications.
        /// </summary>
        IObservable<HeadingUpdatedEvent> UpdatedHeading { get; }

        /// <summary>
        /// Gets the location update notifications.
        /// </summary>
        IObservable<LocationUpdatedEvent> UpdatedLocation { get; }
    }
}