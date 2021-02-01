using System;

namespace Rocket.Surgery.Airframe.Geofence
{
    public class Position : IEquatable<Position>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="lat">The latitude.</param>
        /// <param name="lng">The longitude.</param>
        public Position(double lat, double lng)
        {
            if (lat < -90 || lat > 90)
            {
                throw new ArgumentException($"Invalid latitude value - {lat}");
            }

            if (lng < -180 || lng > 180)
            {
                throw new ArgumentException($"Invalid longitude value - {lng}");
            }

            Latitude = lat;
            Longitude = lng;
        }


        /// <summary>
        /// Gets the latitude.
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Gets the longitude.
        /// </summary>
        public double Longitude { get; }

        public static Position Default { get; } = new Position(0, 0);

        /// <inheritdoc/>
        public override string ToString() => $"Latitude: {Latitude} - Longitude: {Longitude}";

        public bool Equals(Position? other) => other != null && (Latitude, Longitude).Equals((other.Latitude, other.Longitude));

        public static bool operator ==(Position? left, Position? right) => Equals(left, right);

        public static bool operator !=(Position? left, Position? right) => !Equals(left, right);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Position pos && Equals(pos);

        /// <inheritdoc/>
        public override int GetHashCode() => (Latitude, Longitude).GetHashCode();
    }
}