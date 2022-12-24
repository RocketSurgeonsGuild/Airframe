using System;
using System.Reactive;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Interface representing a startup operation.
    /// </summary>
    public interface IStartupOperation : IOperation
    {
        /// <summary>
        /// Starts the operation.
        /// </summary>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Start();
    }
}