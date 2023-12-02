using System;
using System.Reactive;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Interface that represents a reactive CLLocationManager.
    /// </summary>
    public interface ICoreLocationService
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
        IObservable<RegionChangedEvent> DeterminedState { get; }

        /// <summary>
        /// Gets an observable sequence of region beacon failure notifications.
        /// </summary>
        IObservable<RegionBeaconsConstraintFailedEvent> FailedRangingBeacons { get; }

        /// <summary>
        /// Gets an observable sequence of beacon range constraint notifications.
        /// </summary>
        IObservable<RegionBeaconsConstraintRangedEvent> RangedBeaconsSatisfyingConstraint { get; }

        /// <summary>
        /// Gets an observable sequence of regions when the are started being monitored.
        /// </summary>
        IObservable<RegionChangedEvent> StartedMonitoringForRegion { get; }

        /// <summary>
        /// Gets an observable sequence of regions visited.
        /// </summary>
        IObservable<VisitedEvent> Visited { get; }

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
