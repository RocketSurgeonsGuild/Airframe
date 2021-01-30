using System;

namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Notification of a region being visited.
    /// </summary>
    public class VisitedEvent
    {
        public VisitedEvent(CLVisit visit)
        {
            Location = visit.Location;
            ArrivalDate = visit.ArrivalDate;
            DepartureDate = visit.DepartureDate;
            HorizontalAccuracy = visit.HorizontalAccuracy;
        }

        public DateTimeOffset ArrivalDate { get; }
        public Location Location { get; }
        public DateTimeOffset DepartureDate { get; }
        public double HorizontalAccuracy { get; }
    }
}