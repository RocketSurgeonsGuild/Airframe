using System;

namespace Rocket.Surgery.Airframe.Apple
{
    public class GeofenceRegion : IEquatable<GeofenceRegion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeofenceRegion"/> class.
        /// </summary>
        /// <param name="identifier">The region identifier.</param>
        /// <param name="center">The center of the region.</param>
        /// <param name="radius">The radius of the region.</param>
        public GeofenceRegion(string identifier, Position center, double radius)
        {
            Identifier = identifier;
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets the center.
        /// </summary>
        public Position Center { get; }

        /// <summary>
        /// Gets the radius.
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the region should be disposed after exiting.
        /// </summary>
        public bool SingleUse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the region should notify on entry.
        /// </summary>
        public bool NotifyOnEntry { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the region should notify on exit.
        /// </summary>
        public bool NotifyOnExit { get; set; } = true;

        public static bool operator ==(GeofenceRegion left, GeofenceRegion right) => Equals(left, right);

        public static bool operator !=(GeofenceRegion left, GeofenceRegion right) => !Equals(left, right);

        /// <inheritdoc/>
        public override string ToString() => $"[Identifier: {Identifier}]";

        /// <inheritdoc/>
        public bool Equals(GeofenceRegion other) => Identifier == other?.Identifier;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is GeofenceRegion region && Equals(region);

        /// <inheritdoc/>
        public override int GetHashCode() => Identifier?.GetHashCode() ?? 0;
    }
}