using System;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Represents common <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpans
    {
        /// <summary>
        /// Gets the default refresh <see cref="TimeSpan"/>.
        /// </summary>
        public static TimeSpan RefreshInterval { get; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Gets the default <see cref="TimeSpan"/>.
        /// </summary>
        public static TimeSpan Zero { get; } = TimeSpan.Zero;
    }
}