using System;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Represents a time interval.
    /// </summary>
    public readonly struct IntervalTime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalTime"/> struct.
        /// </summary>
        /// <param name="length">The interval time.</param>
        /// <param name="rest">The interval rest.</param>
        public IntervalTime(TimeSpan length, TimeSpan rest)
        {
            Length = length;
            Rest = rest;
        }

        /// <summary>
        /// Gets the interval length.
        /// </summary>
        public TimeSpan Length { get; }

        /// <summary>
        /// Gets the rest.
        /// </summary>
        public TimeSpan Rest { get; }
    }
}