namespace Rocket.Surgery.Airframe.Apple
{
    public class GeofenceRegion : IEquatable<GeofenceRegion>
    {
        public GeofenceRegion(string identifier,
            Position center,
            double radius)
        {
            Identifier = identifier;
            Center = center;
            Radius = radius;
        }

        public string Identifier { get; }
        public Position Center { get; }
        public double Radius { get; }

        public bool SingleUse { get; set; }
        public bool NotifyOnEntry { get; set; } = true;
        public bool NotifyOnExit { get; set; } = true;

 
        public override string ToString() => $"[Identifier: {Identifier}]";
        public bool Equals(GeofenceRegion other) => Identifier == other?.Identifier;
        public override bool Equals(object obj) => obj is GeofenceRegion region && Equals(region);
        public override int GetHashCode() => Identifier?.GetHashCode() ?? 0;

        public static bool operator ==(GeofenceRegion left, GeofenceRegion right) => Equals(left, right);
        public static bool operator !=(GeofenceRegion left, GeofenceRegion right) => !Equals(left, right);
    }
}