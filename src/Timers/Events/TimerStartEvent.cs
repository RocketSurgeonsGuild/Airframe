using System;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Indicates a timer has started.
    /// </summary>
    public class TimerStartEvent : TimerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerStartEvent"/> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public TimerStartEvent(TimeSpan duration) => Duration = duration;

        /// <summary>
        /// Gets the duration of the even that is starting.
        /// </summary>
        public TimeSpan Duration { get; }
    }
}