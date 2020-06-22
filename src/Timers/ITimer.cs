using System;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Interface representing a base timer.
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// Gets the timer event changed event.
        /// </summary>
        IObservable<TimerEvent> Changed { get; }

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