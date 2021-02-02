using System;

namespace Rocket.Surgery.Airframe.Events
{
    /// <summary>
    /// Represents a error event.
    /// </summary>
    public class ErrorEvent
    {
        /// <summary>
        /// Gets an exception.
        /// </summary>
        public Exception Exception { get; }
    }
}