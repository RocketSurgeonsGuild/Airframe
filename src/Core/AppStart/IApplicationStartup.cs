using System;
using System.Reactive;
using System.Reactive.Concurrency;

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
        /// <param name="concurrentOperations">The maximum concurrent operations. </param>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Startup(int concurrentOperations);

        /// <summary>
        /// Starts the application life cycle.
        /// </summary>
        /// <param name="concurrentOperations">The maximum concurrent operations. </param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Startup(int concurrentOperations, IScheduler scheduler);
    }
}