using System;
using CoreLocation;
using Foundation;
using ObjCRuntime;
using Rocket.Surgery.Airframe;

namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Interface representing a <see cref="CLLocationManager"/> at the iOS platform level.
    /// </summary>
    public interface ICLLocationManager
    {
        /// <summary>
        /// Event handler for authorization changes events.
        /// </summary>
        event EventHandler<CLAuthorizationChangedEventArgs> AuthorizationChanged;

        /// <summary>
        /// Event handler for deferred updates finished events.
        /// </summary>
        event EventHandler<NSErrorEventArgs> DeferredUpdatesFinished;

        /// <summary>
        /// Event handler for did determine state events.
        /// </summary>
        event EventHandler<CLRegionStateDeterminedEventArgs> DidDetermineState;

        /// <summary>
        /// Event handler for did fail ranging beacons events.
        /// </summary>
        event EventHandler<CLRegionBeaconsConstraintFailedEventArgs> DidFailRangingBeacons;

        /// <summary>
        /// Event handler for did range beacons satisfy constraint events.
        /// </summary>
        event EventHandler<CLRegionBeaconsConstraintRangedEventArgs> DidRangeBeaconsSatisfyingConstraint;

        /// <summary>
        /// Event handler for did start monitor for region events.
        /// </summary>
        event EventHandler<CLRegionEventArgs> DidStartMonitoringForRegion;

        /// <summary>
        /// Event handler for did visit events.
        /// </summary>
        event EventHandler<CLVisitedEventArgs> DidVisit;

        /// <summary>
        /// Event handler for failed events.
        /// </summary>
        event EventHandler<NSErrorEventArgs> Failed;

        /// <summary>
        /// Event handler for location updates paused events.
        /// </summary>
        event EventHandler LocationUpdatesPaused;

        /// <summary>
        /// Event handler for location updates resumed events.
        /// </summary>
        event EventHandler LocationUpdatesResumed;

        /// <summary>
        /// Event handler for locations updated events.
        /// </summary>
        event EventHandler<CLLocationsUpdatedEventArgs> LocationsUpdated;

        /// <summary>
        /// Event handler for region error events.
        /// </summary>
        event EventHandler<CLRegionErrorEventArgs> MonitoringFailed;

        /// <summary>
        /// Event handler for region entered.
        /// </summary>
        event EventHandler<CLRegionEventArgs> RegionEntered;

        /// <summary>
        /// Event handler for
        /// </summary>
        event EventHandler<CLRegionEventArgs> RegionLeft;

        /// <summary>
        /// Event handler for
        /// </summary>
        event EventHandler<CLHeadingUpdatedEventArgs> UpdatedHeading;

        /// <summary>
        /// Event handler for
        /// </summary>
        event EventHandler<CLLocationUpdatedEventArgs> UpdatedLocation;

        /// <summary>
        /// Dismisses the heading calibration display.
        /// </summary>
        void DismissHeadingCalibrationDisplay();

        /// <summary>
        /// Request the current state.
        /// </summary>
        /// <param name="regionClass">The region class.</param>
        /// <returns>A value indicating whether the <see cref="CLLocationManager"/> can monitor a region.</returns>
        bool IsMonitoringAvailable(Class regionClass);

        /// <summary>
        /// Request the current location.
        /// </summary>
        void RequestLocation();

        /// <summary>
        /// Request the current state.
        /// </summary>
        /// <param name="region">The region.</param>
        void RequestState(CLRegion region);

        /// <summary>
        /// Request to use the location services when application is in use.
        /// </summary>
        void RequestAlwaysAuthorization();

        /// <summary>
        /// Request to use the location services when application is in use.
        /// </summary>
        void RequestWhenInUseAuthorization();

        /// <summary>
        /// Starts monitoring a region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="desiredAccuracy">The requested accuracy.</param>
        void StartMonitoring(CLRegion region, double desiredAccuracy);

        /// <summary>
        /// Starts monitoring a region.
        /// </summary>
        /// <param name="region">The region.</param>
        void StartMonitoring(CLRegion region);

        /// <summary>
        /// Starts monitoring significant location changes.
        /// </summary>
        void StartMonitoringSignificantLocationChanges();

        /// <summary>
        /// Starts monitoring visits.
        /// </summary>
        void StartMonitoringVisits();

        /// <summary>
        /// Starts receiving updates on heading.
        /// </summary>
        void StartUpdatingHeading();

        /// <summary>
        /// Starts receiving updates on location.
        /// </summary>
        void StartUpdatingLocation();

        /// <summary>
        /// Stops monitoring the specified region.
        /// </summary>
        /// <param name="region">The region.</param>
        void StopMonitoring(CLRegion region);

        /// <summary>
        /// Stops receiving updates on significant location changes.
        /// </summary>
        void StopMonitoringSignificantLocationChanges();

        /// <summary>
        /// Stops receiving updates on visits.
        /// </summary>
        void StopMonitoringVisits();

        /// <summary>
        /// Stops receiving updates on heading.
        /// </summary>
        void StopUpdatingHeading();

        /// <summary>
        /// Stops receiving updates on location.
        /// </summary>
        void StopUpdatingLocation();
    }
}
