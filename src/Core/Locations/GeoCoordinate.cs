using System;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Represents a coordinate.
/// </summary>
/// <remarks>any of a set of numbers used in specifying the location of a point on a line, on a surface, or in space.</remarks>
public class GeoCoordinate : IEquatable<GeoCoordinate>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeoCoordinate"/> class.
    /// </summary>
    /// <param name="latitude">The latitude value.</param>
    /// <param name="longitude">The longitude value.</param>
    public GeoCoordinate(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
        {
            throw new ArgumentException($"Invalid latitude value - {latitude}");
        }

        if (longitude is < -180 or > 180)
        {
            throw new ArgumentException($"Invalid longitude value - {longitude}");
        }

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Gets the default <see cref="GeoCoordinate"/>.
    /// </summary>
    public static GeoCoordinate Default { get; } = new GeoCoordinate(0, 0);

    /// <summary>
    /// Gets the latitude of the coordinate.
    /// </summary>
    public double Latitude { get; }

    /// <summary>
    /// Gets the longitude of the coordinate.
    /// </summary>
    public double Longitude { get; }

    public static bool operator ==(GeoCoordinate? left, GeoCoordinate? right) => Equals(left, right);

    public static bool operator !=(GeoCoordinate? left, GeoCoordinate? right) => !Equals(left, right);

    /// <inheritdoc/>
    public override string ToString() => $"Latitude: {Latitude} - Longitude: {Longitude}";

    /// <inheritdoc/>
    public bool Equals(GeoCoordinate? other) => other != null && (Latitude, Longitude).Equals((other.Latitude, other.Longitude));

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is GeoCoordinate coordinate && Equals(coordinate);

    /// <inheritdoc/>
    public override int GetHashCode() => (Latitude, Longitude).GetHashCode();
}