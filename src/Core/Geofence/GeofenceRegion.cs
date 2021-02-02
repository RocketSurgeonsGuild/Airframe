using System;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents a region where a geofence exists.
    /// </summary>
    public class GeofenceRegion : IEquatable<GeofenceRegion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeofenceRegion"/> class.
        /// </summary>
        /// <param name="identifier">The region identifier.</param>
        /// <param name="origin">The center of the region.</param>
        /// <param name="radius">The radius of the region.</param>
        public GeofenceRegion(string identifier, Position origin, double radius)
        {
            Identifier = identifier;
            Origin = origin;
            Radius = radius;
        }

        /// <summary>
        /// Gets the default <see cref="GeoRegion"/>.
        /// </summary>
        public static GeofenceRegion Default { get; } = new GeofenceRegion(nameof(Default), Position.Default, 0);

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets the center.
        /// </summary>
        public Position Origin { get; }

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