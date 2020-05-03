using System;

namespace Rocket.Surgery.Airframe.Apple
{
    /// <summary>
    /// Returns the entire state of the current Core Location Manager.
    /// </summary>
    public class MonitorResult
    {
        /// <summary>
        /// Gets the current state.
        /// </summary>
        public MonitorState State { get; }

        /// <summary>
        /// Gets the last changed date time offset.
        /// </summary>
        public DateTimeOffset LastChanged { get; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the result of the request was successful.
        /// </summary>
        public bool Success { get; set; }
    }
}