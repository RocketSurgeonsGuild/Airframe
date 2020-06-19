using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.Timers
{
    public abstract class TimerBase : ReactiveObject, IDisposable
    {
        protected readonly CompositeDisposable TimesUp = new CompositeDisposable();
        protected readonly Subject<bool> Running = new Subject<bool>();
        protected readonly IScheduler Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerBase"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        protected TimerBase(IScheduler scheduler)
        {
            Scheduler = scheduler;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether it is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                TimesUp?.Dispose();
                Running?.Dispose();
            }
        }
    }
}