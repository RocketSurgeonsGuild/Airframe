using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Default Exception handler for Reactive Application.
    /// </summary>
    public class DefaultExceptionHandler : IObserver<Exception>
    {
        /// <inheritdoc/>
        public virtual void OnNext(Exception value)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            RxApp.MainThreadScheduler.Schedule(() => throw value);
        }

        /// <inheritdoc/>
        public virtual void OnError(Exception error)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            RxApp.MainThreadScheduler.Schedule(() => throw error);
        }

        /// <inheritdoc/>
        public virtual void OnCompleted()
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            RxApp.MainThreadScheduler.Schedule(() => throw new NotImplementedException());
        }
    }
}