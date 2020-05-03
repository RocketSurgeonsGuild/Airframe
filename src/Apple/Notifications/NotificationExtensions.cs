using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using CoreLocation;
using Foundation;

namespace Rocket.Surgery.Airframe.Apple.Notifications
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
        public static LocationUpdatedNotification ToNotification(CLLocationUpdatedEventArgs args) =>
            new LocationUpdatedNotification
            {
                Previous = args.OldLocation.Coordinate.ToLocation(),
                Current = args.NewLocation.Coordinate.ToLocation()
            };

        /// <summary>
        /// Converts the <see cref="CLRegionEventArgs"/> to <see cref="RegionChangedNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionChangedNotification ToNotification(this CLRegionEventArgs args) =>
            new RegionChangedNotification
            {
                Region = args.Region.ToGeoRegion()
            };

        /// <summary>
        /// Converts the <see cref="CLRegionStateDeterminedEventArgs"/> to <see cref="RegionChangedNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionChangedNotification ToNotification(this CLRegionStateDeterminedEventArgs args) =>
            new RegionChangedNotification
            {
                Region = args.Region.ToGeoRegion(),
                State = RegionStates[args.State]
            };

        /// <summary>
        /// Converts the <see cref="CLRegionStateDeterminedEventArgs"/> to <see cref="ErrorNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static ErrorNotification ToNotification(this NSErrorEventArgs args) =>
            new ErrorNotification { };

        /// <summary>
        /// Converts the <see cref="CLVisitedEventArgs"/> to <see cref="VisitedNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static VisitedNotification ToNotification(this CLVisitedEventArgs args) =>
            new VisitedNotification { };

        /// <summary>
        /// Converts the <see cref="CLRegionErrorEventArgs"/> to <see cref="RegionErrorNotification"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The notification.</returns>
        public static RegionErrorNotification ToNotification(this CLRegionErrorEventArgs args) =>
            new RegionErrorNotification { };

        /// <summary>
        /// Converts the <see cref="CLRegionErrorEventArgs"/> to <see cref="RegionErrorNotification"/>.
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
        /// Converts the <see cref="CLLocationCoordinate2D"/> to a <see cref="Location"/>.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The converted vale.</returns>
        public static Location ToLocation(this CLLocationCoordinate2D location) => new Location(location.Latitude, location.Longitude);
    }
}