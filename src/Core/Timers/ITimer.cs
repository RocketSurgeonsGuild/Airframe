using System;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Interface representing a base timer.
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the timer at the specified time.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        void Start(TimeSpan timeSpan);

        /// <summary>
        /// Starts the timer from it's current time.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        void Stop();
    }
}