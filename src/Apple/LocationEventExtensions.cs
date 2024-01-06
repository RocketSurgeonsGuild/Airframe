using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;
using Foundation;

namespace Rocket.Surgery.Airframe.Apple
{
    internal static class LocationEventExtensions
    {
        private static readonly Dictionary<CLAuthorizationStatus, AuthorizationStatus> AuthorizationStatuses =
            new()
            {
                { CLAuthorizationStatus.NotDetermined, AuthorizationStatus.NotDetermined },
                { CLAuthorizationStatus.Restricted, AuthorizationStatus.Restricted },
                { CLAuthorizationStatus.Denied, AuthorizationStatus.Denied },
                { CLAuthorizationStatus.AuthorizedAlways, AuthorizationStatus.AuthorizedAlways },
                { CLAuthorizationStatus.AuthorizedWhenInUse, AuthorizationStatus.AuthorizedWhenInUse }
            };

        private static readonly Dictionary<CLRegionState, RegionState> RegionStates =
            new()
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
            new(AuthorizationStatuses[args.Status]);

        /// <summary>
        /// Converts the <see cref="CLHeadingUpdatedEventArgs"/> to an instance of <see cref="HeadingUpdatedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The changed notification.</returns>
        public static HeadingUpdatedEvent ToNotification(this CLHeadingUpdatedEventArgs args) =>
            new();

        /// <summary>
        /// Converts the <see cref="CLLocationsUpdatedEventArgs"/> to <see cref="LocationsUpdatedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static LocationsUpdatedEvent ToNotification(this CLLocationsUpdatedEventArgs args) =>
            new(args.Locations.Select(location => location.ToGpsLocation()));

        /// <summary>
        /// Converts the <see cref="CLLocationsUpdatedEventArgs"/> to <see cref="LocationsUpdatedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static LocationUpdatedEvent ToNotification(this CLLocationUpdatedEventArgs args) =>
            new(
                args.OldLocation.ToGpsLocation(),
                args.NewLocation.ToGpsLocation());

        /// <summary>
        /// Converts the <see cref="CLRegionEventArgs"/> to <see cref="RegionChangedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionChangedEvent ToNotification(this CLRegionEventArgs args) =>
            new(args.Region.ToGeoRegion());

        /// <summary>
        /// Converts the <see cref="CLRegionStateDeterminedEventArgs"/> to <see cref="RegionChangedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionChangedEvent ToNotification(this CLRegionStateDeterminedEventArgs args) =>
            new(args.Region.ToGeoRegion(), RegionStates[args.State]);

        /// <summary>
        /// Converts the <see cref="NSErrorEventArgs"/> to <see cref="ErrorEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static ErrorEvent ToNotification(this NSErrorEventArgs args) =>
            new(new Exception(args.ToString()));

        /// <summary>
        /// Converts the <see cref="CLVisitedEventArgs"/> to <see cref="VisitedEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static VisitedEvent ToNotification(this CLVisitedEventArgs args) =>
            new(args.Visit.Coordinate.ToLocation(), args.Visit.ArrivalDate.ToLocalTime(), args.Visit.DepartureDate.ToLocalTime(), args.Visit.HorizontalAccuracy);

        /// <summary>
        /// Converts the <see cref="CLRegionErrorEventArgs"/> to <see cref="RegionErrorEvent"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionErrorEvent ToNotification(this CLRegionErrorEventArgs args) =>
            new(/*args.Error*/new Exception(),  ToGeoRegion(args.Region));

        /// <summary>
        /// Converts a <see cref="CLRegion"/> to a <see cref="GeoRegion"/>.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <returns>The converted value.</returns>
        public static GeoRegion ToGeoRegion(this CLRegion region) => new(
            region.Identifier,
            region.Center.ToLocation(),
            region.Radius,
            region.NotifyOnEntry,
            region.NotifyOnExit);

        /// <summary>
        /// Converts the <see cref="CLLocationCoordinate2D"/> to a <see cref="GeoLocation"/>.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The converted vale.</returns>
        public static GeoCoordinate ToLocation(this CLLocationCoordinate2D location) => new(location.Latitude, location.Longitude);

        public static IGpsLocation ToGpsLocation(this CLLocation location) => new GpsLocation(location.Coordinate.Latitude, location.Coordinate.Longitude, location.Altitude, location.Course, location.CourseAccuracy, location.Speed, location.SpeedAccuracy, 0, location.Timestamp.ToLocalTime());

        public static DateTime ToLocalTime(this NSDate nsDate)
        {
            DateTime referenceDate = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0));

            return referenceDate.AddSeconds(nsDate.SecondsSinceReferenceDate);
        }
    }
}
