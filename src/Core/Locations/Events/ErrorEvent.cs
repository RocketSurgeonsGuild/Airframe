using System;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents a error event.
    /// </summary>
    public class ErrorEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEvent"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public ErrorEvent(Exception exception) => Exception = exception;

        /// <summary>
        /// Gets an exception.
        /// </summary>
        public Exception Exception { get; }
    }
}