using System;

namespace Rocket.Surgery.Airframe.Apple
{
    public class Position : IEquatable<Position>
    {
        public Position(double lat, double lng)
        {
            if (lat < -90 || lat > 90)
                throw new ArgumentException($"Invalid latitude value - {lat}");

            if (lng < -180 || lng > 180)
                throw new ArgumentException($"Invalid longitude value - {lng}");

            Latitude = lat;
            Longitude = lng;
        }


        public double Latitude { get; }
        public double Longitude { get; }
        

        public override string ToString() => $"Latitude: {Latitude} - Longitude: {Longitude}";
        public bool Equals(Position? other) => other != null && (Latitude, Longitude).Equals((other.Latitude, other.Longitude));
        public static bool operator ==(Position? left, Position? right) => Equals(left, right);
        public static bool operator !=(Position? left, Position? right) => !Equals(left, right);
        public override bool Equals(object obj) => obj is Position pos && Equals(pos);
        public override int GetHashCode() => (Latitude, Longitude).GetHashCode();
    }
}