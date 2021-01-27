using System;
using System.Reactive;

namespace Rocket.Surgery.Airframe.AppStart
{
    /// <summary>
    /// Interface representing that application startup sequence.
    /// </summary>
    public interface IApplicationStartup
    {
        /// <summary>
        /// Gets a value indicating whether the startup is complete.
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Starts the application life cycle.
        /// </summary>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Startup();
    }
}