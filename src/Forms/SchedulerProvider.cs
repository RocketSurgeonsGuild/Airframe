using System.Reactive.Concurrency;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.Forms
{
    /// <summary>
    /// A <see cref="ISchedulerProvider"/> for Xamarin.Forms.
    /// </summary>
    public sealed class SchedulerProvider : ISchedulerProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerProvider"/> class.
        /// </summary>
        public SchedulerProvider()
            : this(RxApp.MainThreadScheduler, RxApp.TaskpoolScheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerProvider"/> class.
        /// </summary>
        /// <param name="mainThread">The main scheduler.</param>
        /// <param name="backgroundThread">The background scheduler.</param>
        public SchedulerProvider(IScheduler mainThread, IScheduler backgroundThread)
        {
            UserInterfaceThread = mainThread;
            BackgroundThread = backgroundThread;
        }

        /// <inheritdoc />
        public IScheduler UserInterfaceThread { get; }

        /// <inheritdoc/>
        public IScheduler BackgroundThread { get; }
    }
}