using System;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Interface representing a gps location.
/// </summary>
public interface IGpsLocation
{
    /// <summary>
    /// Gets the altitude of the reading.
    /// </summary>
    double Altitude { get; }

    /// <summary>
    /// Gets the heading of the reading.
    /// </summary>
    double Heading { get; }

    /// <summary>
    /// Gets the accuracy of the heading.
    /// </summary>
    double HeadingAccuracy { get; }

    /// <summary>
    /// Gets the current speed.
    /// </summary>
    double Speed { get; }

    /// <summary>
    /// Gets the accuracy in meters per second for the speed.
    /// </summary>
    double SpeedAccuracy { get; }

    /// <summary>
    /// Gets the position of the reading.
    /// </summary>
    GeoCoordinate Coordinate { get; }

    /// <summary>
    /// Gets the position accuracy.
    /// </summary>
    double PositionAccuracy { get; }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    DateTimeOffset Timestamp { get; }
}