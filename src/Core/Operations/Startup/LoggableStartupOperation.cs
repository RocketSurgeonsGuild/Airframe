using System;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents a <see cref="IStartupOperation"/> that logs using <see cref="ILogger{TCategoryName}"/>.
    /// </summary>
    public abstract class LoggableStartupOperation : IStartupOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggableStartupOperation"/> class.
        /// </summary>
        /// <param name="factory">The logger factory.</param>
        protected LoggableStartupOperation(ILoggerFactory factory) => Logger = factory.CreateLogger(GetType());

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        IObservable<Unit> IStartupOperation.Start() =>

            // Add logging.
            Start().Finally(() => Logger.LogTrace("{StartupOperation}: Completed", GetType().Name));

        /// <inheritdoc/>
        IObservable<Unit> IOperation<Unit>.Execute() =>

            // Add logging.
            ((IStartupOperation)this).Start();

        /// <inheritdoc/>
        bool ICanExecute.CanExecute()
        {
            var canExecute = CanExecute();
            Logger.LogTrace("Can Execute: {CanExecute}", canExecute);
            return canExecute;
        }

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