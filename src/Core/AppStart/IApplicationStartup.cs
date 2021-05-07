using System;
using System.Reactive;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Interface representing that application startup sequence.
    /// </summary>
    public interface IApplicationStartup
    {
        /// <summary>
        /// Starts the application life cycle.
        /// </summary>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Startup();
    }
}