using System;

namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Notification of a region being visited.
    /// </summary>
    public class VisitedNotification
    {
        public DateTimeOffset ArrivalDate { get; set; }
        public Location Location { get; set; }
        public DateTimeOffset DepartureDate { get; set; }
        public double HorizontalAccuracy { get; set; }
    }
}