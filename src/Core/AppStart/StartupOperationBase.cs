using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Rocket.Surgery.Airframe.AppStart
{
    /// <summary>
    /// Represents a base startup operation.
    /// </summary>
    public abstract class StartupOperationBase : IStartupOperation
    {
        /// <inheritdoc/>
        IObservable<Unit> IStartupOperation.Start()
        {
            // Add logging.
            return Start();
        }

        /// <inheritdoc/>
        bool IStartupOperation.CanExecute() => CanExecute();

        /// <summary>
        /// Template method for the startup operation.
        /// </summary>
        /// <returns>A completion notification.</returns>
        protected abstract IObservable<Unit> Start();

        /// <summary>
        /// Template method for whether or not the startup operation will execute.
        /// </summary>
        /// <returns>A completion notification.</returns>
        protected virtual bool CanExecute() => true;
    }
}