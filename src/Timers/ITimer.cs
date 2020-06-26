using System;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Interface representing a base timer.
    /// </summary>
    public interface ITimer : IObservable<TimeSpan>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        void Stop();
    }
}