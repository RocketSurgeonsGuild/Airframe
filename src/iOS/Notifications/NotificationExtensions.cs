using System.Collections.Generic;
using System.Linq;
using CoreLocation;

namespace Rocket.Surgery.Airframe.iOS.Notifications
{
    public static class NotificationExtensions
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
        /// Converts the <see cref="CLAuthorizationChangedEventArgs"/> to an instance of <see cref="AuthorizationChangedNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The changed notification.</returns>
        public static AuthorizationChangedNotification ToNotification(this CLAuthorizationChangedEventArgs args) =>
            new AuthorizationChangedNotification { Status = AuthorizationStatuses[args.Status] };

        /// <summary>
        /// Converts the <see cref="CLLocationsUpdatedEventArgs"/> to <see cref="LocationsUpdatedNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static LocationsUpdatedNotification ToNotification(this CLLocationsUpdatedEventArgs args) =>
            new LocationsUpdatedNotification { Locations = args.Locations.Select(x => new Location(x.Coordinate.Latitude, x.Coordinate.Longitude)) };

        /// <summary>
        /// Converts the <see cref="CLLocationsUpdatedEventArgs"/> to <see cref="LocationsUpdatedNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static LocationUpdatedNotification ToNotification(this CLLocationUpdatedEventArgs args) =>
            new LocationUpdatedNotification
            {
                Previous = new Location(args.OldLocation.Coordinate.Latitude, args.OldLocation.Coordinate.Longitude),
                Current = new Location(args.OldLocation.Coordinate.Latitude, args.OldLocation.Coordinate.Longitude)
            };

        /// <summary>
        /// Converts the <see cref="CLRegionEventArgs"/> to <see cref="RegionNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionNotification ToNotification(this CLRegionEventArgs args) =>
            new RegionNotification
            {
                Region = args.Region.ToGeoRegion()
            };

        /// <summary>
        /// Converts the <see cref="CLRegionEventArgs"/> to <see cref="RegionNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionStateDeterminedNotification ToNotification(this CLRegionStateDeterminedEventArgs args) =>
            new RegionStateDeterminedNotification
            {
                Region = args.Region.ToGeoRegion(),
                State = RegionStates[args.State]
            };

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
        /// Converts the <see cref="CLLocationCoordinate2D"/> to a <see cref="Location"/>.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The converted vale.</returns>
        public static Location ToLocation(this CLLocationCoordinate2D location) => new Location(location.Latitude, location.Longitude);
    }
}