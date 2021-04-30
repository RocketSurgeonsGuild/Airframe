using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Represents a base abstraction for a timer.
    /// </summary>
    public abstract class TimerBase : ReactiveObject, IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TimerBase" /> class.
        /// </summary>
        /// <param name="schedulerProvider">The scheduler.</param>
        protected TimerBase(ISchedulerProvider schedulerProvider) => SchedulerProvider = schedulerProvider;

        /// <inheritdoc />
        protected ISchedulerProvider SchedulerProvider { get; set; }

        /// <inheritdoc />
        protected Subject<bool> Running { get; } = new Subject<bool>();

        /// <inheritdoc />
        protected CompositeDisposable TimesUp { get; } = new CompositeDisposable();

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposes of resources.
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