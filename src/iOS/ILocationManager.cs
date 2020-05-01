using System;
using CoreLocation;
using Foundation;

namespace Rocket.Surgery.Airframe.iOS
{
    public interface ILocationManager
    {
        event EventHandler<CLAuthorizationChangedEventArgs> AuthorizationChanged;
        event EventHandler<NSErrorEventArgs> DeferredUpdatesFinished;
        event EventHandler<CLRegionStateDeterminedEventArgs> DidDetermineState;
        event EventHandler<CLRegionBeaconsConstraintFailedEventArgs> DidFailRangingBeacons;
        event EventHandler<CLRegionBeaconsRangedEventArgs> DidRangeBeacons;
        event EventHandler<CLRegionBeaconsConstraintRangedEventArgs> DidRangeBeaconsSatisfyingConstraint;
        event EventHandler<CLRegionEventArgs> DidStartMonitoringForRegion;
        event EventHandler<CLVisitedEventArgs> DidVisit;
        event EventHandler<NSErrorEventArgs> Failed;
        event EventHandler LocationUpdatesPaused;
        event EventHandler LocationUpdatesResumed;
        event EventHandler<CLLocationsUpdatedEventArgs> LocationsUpdated;
        event EventHandler<CLRegionErrorEventArgs> MonitoringFailed;
        event EventHandler<CLRegionBeaconsFailedEventArgs> RangingBeaconsDidFailForRegion;
        event EventHandler<CLRegionEventArgs> RegionEntered;
        event EventHandler<CLRegionEventArgs> RegionLeft;
        event EventHandler<CLHeadingUpdatedEventArgs> UpdatedHeading;
        event EventHandler<CLLocationUpdatedEventArgs> UpdatedLocation;
    }
}