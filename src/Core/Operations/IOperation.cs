using System;
using System.Reactive;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Interface representing a base operation.
    /// </summary>
    public interface IOperation : IOperation<Unit>
    {
    }

    /// <summary>
    /// Interface representing a base operation.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public interface IOperation<T> : ICanExecute
    {
        /// <summary>
        /// Executes the operation.
        /// </summary>
        /// <returns>A completion notification.</returns>
        IObservable<T> Execute();
    }
}