using System;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Represents a gps location.
/// </summary>
public class GpsLocation : IGpsLocation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GpsLocation"/> class.
    /// </summary>
    /// <param name="latitude">The latitude.</param>
    /// <param name="longitude">The longitude.</param>
    /// <param name="altitude">The altitude.</param>
    /// <param name="heading">The heading.</param>
    /// <param name="headingAccuracy">The heading accuracy.</param>
    /// <param name="speed">The speed.</param>
    /// <param name="speedAccuracy">The speed accuracy.</param>
    /// <param name="positionAccuracy">The position accuracy.</param>
    /// <param name="timestamp">The date time of the reading.</param>
    public GpsLocation(
        double latitude,
        double longitude,
        double altitude,
        double heading,
        double headingAccuracy,
        double speed,
        double speedAccuracy,
        double positionAccuracy,
        DateTimeOffset timestamp)
        : this(new GeoCoordinate(latitude, longitude), altitude, heading, headingAccuracy, speed, speedAccuracy, positionAccuracy, timestamp)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GpsLocation"/> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="altitude">The altitude.</param>
    /// <param name="heading">The heading.</param>
    /// <param name="headingAccuracy">The heading accuracy.</param>
    /// <param name="speed">The speed.</param>
    /// <param name="speedAccuracy">The speed accuracy.</param>
    /// <param name="positionAccuracy">The position accuracy.</param>
    /// <param name="timestamp">The date time of the reading.</param>
    public GpsLocation(
        GeoCoordinate position,
        double altitude,
        double heading,
        double headingAccuracy,
        double speed,
        double speedAccuracy,
        double positionAccuracy,
        DateTimeOffset timestamp)
    {
        Altitude = altitude;
        Heading = heading;
        HeadingAccuracy = headingAccuracy;
        Speed = speed;
        SpeedAccuracy = speedAccuracy;
        Position = position;
        PositionAccuracy = positionAccuracy;
        Timestamp = timestamp;
    }

    /// <inheritdoc/>
    public double Altitude { get; }

    /// <inheritdoc/>
    public double Heading { get; }

    /// <inheritdoc/>
    public double HeadingAccuracy { get; }

    /// <inheritdoc/>
    public double Speed { get; }

    /// <inheritdoc/>
    public double SpeedAccuracy { get; }

    /// <inheritdoc/>
    public GeoCoordinate Position { get; }

    /// <inheritdoc/>
    public double PositionAccuracy { get; }

    /// <inheritdoc/>
    public DateTimeOffset Timestamp { get; }
}