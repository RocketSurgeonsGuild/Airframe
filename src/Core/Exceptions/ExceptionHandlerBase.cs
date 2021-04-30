using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using JetBrains.Annotations;

namespace Rocket.Surgery.Airframe.Exceptions
{
    /// <summary>
    /// Represents a base <see cref="IExceptionHandler"/> implementation.
    /// </summary>
    public abstract class ExceptionHandlerBase : IExceptionHandler
    {
        private readonly IScheduler _scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerBase"/> class.
        /// </summary>
        /// <param name="scheduler">The main thread scheduler.</param>
        protected ExceptionHandlerBase([NotNull] IScheduler scheduler) => _scheduler = scheduler;

        /// <inheritdoc/>
        void IObserver<Exception>.OnCompleted()
        {
            DebuggerBreak();

            OnCompleted();

            _scheduler.Schedule(() => throw new NotImplementedException());
        }

        /// <inheritdoc/>
        void IObserver<Exception>.OnError(Exception error)
        {
            DebuggerBreak();

            OnError(error);

            _scheduler.Schedule(() => throw error);
        }

        /// <inheritdoc/>
        void IObserver<Exception>.OnNext(Exception value)
        {
            DebuggerBreak();

            OnNext(value);

            _scheduler.Schedule(() => throw value);
        }

        /// <summary>
        /// Notifies the observer that the provider has finished sending push based notifications.
        /// </summary>
        protected virtual void OnCompleted()
        {
        }

        /// <summary>
        /// Notifies the observer that the provider has experienced and error.
        /// </summary>
        /// <param name="error">The exception.</param>
        protected virtual void OnError(Exception error)
        {
        }

        /// <summary>
        /// Provides the observer with the next exception.
        /// </summary>
        /// <param name="value">The exception.</param>
        protected virtual void OnNext(Exception value)
        {
        }

        private void DebuggerBreak()
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
    }
}