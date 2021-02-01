using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using CoreLocation;
using Foundation;
using Rocket.Surgery.Airframe.Apple.Notifications;

namespace Rocket.Surgery.Airframe.Apple
{
    public static class LocationEventExtensions
    {
        private static readonly Dictionary<CLAuthorizationStatus, AuthorizationStatus> AuthorizationStatuses =
            new Dictionary<CLAuthorizationStatus, AuthorizationStatus>
            {
                { CLAuthorizationStatus.NotDetermined, AuthorizationStatus.NotDetermined },
                { CLAuthorizationStatus.Restricted, AuthorizationStatus.Restricted },
                { CLAuthorizationStatus.Denied, AuthorizationStatus.Denied },
                { CLAuthorizationStatus.AuthorizedAlways, AuthorizationStatus.AuthorizedAlways },
                { CLAuthorizationStatus.AuthorizedWhenInUse, AuthorizationStatus.AuthorizedWhenInUse }
            };

        private static readonly Dictionary<CLRegionState, RegionState> RegionStates =
            new Dictionary<CLRegionState, RegionState>
            {
                { CLRegionState.Unknown, RegionState.Unknown },
                { CLRegionState.Inside, RegionState.Inside },
                { CLRegionState.Outside, RegionState.Outside }
            };

        /// <summary>
        /// Converts the <see cref="CLAuthorizationChangedEventArgs"/> to an instance of <see cref="AuthorizationChangedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The changed notification.</returns>
        public static AuthorizationChangedEvent ToNotification(this CLAuthorizationChangedEventArgs args) =>
            new AuthorizationChangedEvent(AuthorizationStatuses[args.Status]);

        /// <summary>
        /// Converts the <see cref="CLAuthorizationChangedEventArgs"/> to an instance of <see cref="AuthorizationChangedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The changed notification.</returns>
        public static RegionBeaconsConstraintFailedEvent ToNotification(
            this CLRegionBeaconsConstraintFailedEventArgs args) => new RegionBeaconsConstraintFailedEvent();

        /// <summary>
        /// Converts the <see cref="CLRegionBeaconsRangedEventArgs"/> to an instance of <see cref="RegionBeaconRangedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The changed notification.</returns>
        public static RegionBeaconRangedEvent ToNotification(this CLRegionBeaconsRangedEventArgs args) =>
            new RegionBeaconRangedEvent();

        /// <summary>
        /// Converts the <see cref="CLRegionBeaconsConstraintRangedEventArgs"/> to an instance of <see cref="RegionBeaconRangedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The changed notification.</returns>
        public static RegionBeaconsConstraintRangedNotification ToNotification(this CLRegionBeaconsConstraintRangedEventArgs args) =>
            new RegionBeaconsConstraintRangedNotification();

        /// <summary>
        /// Converts the <see cref="CLRegionBeaconsFailedEventArgs"/> to an instance of <see cref="RegionBeaconsFailedNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The changed notification.</returns>
        public static RegionBeaconsFailedNotification ToNotification(this CLRegionBeaconsFailedEventArgs args) =>
            new RegionBeaconsFailedNotification();

        /// <summary>
        /// Converts the <see cref="CLHeadingUpdatedEventArgs"/> to an instance of <see cref="RegionBeaconsFailedNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The changed notification.</returns>
        public static HeadingUpdatedEvent ToNotification(this CLHeadingUpdatedEventArgs args) =>
            new HeadingUpdatedEvent();

        /// <summary>
        /// Converts the <see cref="CLLocationsUpdatedEventArgs"/> to <see cref="LocationsUpdatedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static LocationsUpdatedEvent ToNotification(this CLLocationsUpdatedEventArgs args) =>
            new LocationsUpdatedEvent(args.Locations.Select(x => new GeoLocation(x.Coordinate.Latitude, x.Coordinate.Longitude)));

        /// <summary>
        /// Converts the <see cref="CLLocationsUpdatedEventArgs"/> to <see cref="LocationsUpdatedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static LocationUpdatedEvent ToNotification(CLLocationUpdatedEventArgs args) =>
            new LocationUpdatedEvent(args.OldLocation.Coordinate.ToLocation(),
                args.NewLocation.Coordinate.ToLocation());

        /// <summary>
        /// Converts the <see cref="CLRegionEventArgs"/> to <see cref="RegionChangedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionChangedEvent ToNotification(this CLRegionEventArgs args) =>
            new RegionChangedEvent(args.Region.ToGeoRegion());

        /// <summary>
        /// Converts the <see cref="CLRegionStateDeterminedEventArgs"/> to <see cref="RegionChangedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionChangedEvent ToNotification(this CLRegionStateDeterminedEventArgs args) =>
            new RegionChangedEvent(args.Region.ToGeoRegion(), RegionStates[args.State]);

        /// <summary>
        /// Converts the <see cref="CLRegionStateDeterminedEventArgs"/> to <see cref="ErrorNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static ErrorNotification ToNotification(this NSErrorEventArgs args) =>
            new ErrorNotification { };

        /// <summary>
        /// Converts the <see cref="CLVisitedEventArgs"/> to <see cref="VisitedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static VisitedEvent ToNotification(this CLVisitedEventArgs args) =>
            new VisitedEvent(args.Visit);

        /// <summary>
        /// Converts the <see cref="CLRegionErrorEventArgs"/> to <see cref="RegionErrorEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionErrorEvent ToNotification(this CLRegionErrorEventArgs args) =>
            new RegionErrorEvent(args.Error,  ToGeoRegion(args.Region));

        /// <summary>
        /// Converts the <see cref="CLRegionErrorEventArgs"/> to <see cref="RegionErrorEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="obj">The object.</param>
        /// <returns>The notification.</returns>
        public static Unit ToNotification(this object obj) => Unit.Default;

        /// <summary>
        /// Converts a <see cref="CLRegion"/> to a <see cref="GeoRegion"/>.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <returns>The converted value.</returns>
        public static GeoRegion ToGeoRegion(this CLRegion region) =>
            new GeoRegion
            {
                Identifier = region.Identifier,
                Center = region.Center.ToLocation(),
                Radius = region.Radius,
                NotifyOnEntry = region.NotifyOnEntry,
                NotifyOnExit = region.NotifyOnExit
            };

        /// <summary>
        /// Converts the <see cref="CLLocationCoordinate2D"/> to a <see cref="GeoLocation"/>.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The converted vale.</returns>
        public static GeoLocation ToLocation(this CLLocationCoordinate2D location) => new GeoLocation(location.Latitude, location.Longitude);

        public static DateTime ToLocalTime(this NSDate nsDate)
        {
            DateTime referenceDate = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0));

            return referenceDate.AddSeconds(nsDate.SecondsSinceReferenceDate);
        }
    }
}