using System;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Notification of a region being visited.
/// </summary>
public class VisitedEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VisitedEvent"/> class.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="arrivalDate">The arrival date.</param>
    /// <param name="departureDate">The departure date.</param>
    /// <param name="horizontalAccuracy">The accuracy.</param>
    public VisitedEvent(GeoCoordinate location, DateTimeOffset arrivalDate, DateTimeOffset departureDate, double horizontalAccuracy)
    {
        Location = location;
        ArrivalDate = arrivalDate;
        DepartureDate = departureDate;
        HorizontalAccuracy = horizontalAccuracy;
    }

    /// <summary>
    /// Gets the arrival date.
    /// </summary>
    public DateTimeOffset ArrivalDate { get; }

    /// <summary>
    /// Gets the geo location.
    /// </summary>
    public GeoCoordinate Location { get; }

    /// <summary>
    /// Gets the departure date.
    /// </summary>
    public DateTimeOffset DepartureDate { get; }

    /// <summary>
    /// Gets the horizontal accuracy.
    /// </summary>
    public double HorizontalAccuracy { get; }
}