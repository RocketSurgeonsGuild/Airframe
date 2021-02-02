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
        {
            UserInterfaceThread = RxApp.MainThreadScheduler;
            BackgroundThread = RxApp.TaskpoolScheduler;
        }

        /// <inheritdoc />
        public IScheduler UserInterfaceThread { get; }

        /// <inheritdoc/>
        public IScheduler BackgroundThread { get; }
    }
}