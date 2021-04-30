using System;
using System.Reactive.Linq;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    ///     A timer that partitions time base on a provided partition number..
    /// </summary>
    public class DivisibleTimer : TimerBase
    {
        private ObservableTimer _currentTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisibleTimer"/> class.
        /// </summary>
        /// <param name="schedulerProvider">The scheduler.</param>
        public DivisibleTimer(ISchedulerProvider schedulerProvider)
            : base(schedulerProvider)
        {
            Interval = Observable.Empty<TimeSpan>();
            Timer = Observable.Empty<TimeSpan>();
        }

        /// <summary>
        /// Gets the interval timer.
        /// </summary>
        public IObservable<TimeSpan> Interval { get; private set; }

        /// <summary>
        /// Gets the over all timer.
        /// </summary>
        public IObservable<TimeSpan> Timer { get; private set; }

        /// <summary>
        /// Gets the interval time.
        /// </summary>
        public TimeSpan IntervalTime { get; private set; }

        /// <summary>
        /// Started the timer.
        /// </summary>
        /// <param name="partition">The partition.</param>
        /// <param name="duration">The overall duration.</param>
        public void Start(int partition, TimeSpan duration)
        {
            var refreshInterval = TimeSpan.FromMilliseconds(1000);

            long ticks = Math.DivRem(duration.Ticks, partition, out var remainder);

            TimeSpan timePerPartition = TimeSpan.FromTicks(ticks);

            Timer =
                Observable
                    .Range(0, partition)
                    .Select(_ =>
                    {
                        _currentTimer = new ObservableTimer(SchedulerProvider);
                        return Observable.Empty<TimeSpan>();
                    })
                    .Concat();

            IntervalTime = timePerPartition;

            // TODO: [rlittlesii: January 11, 2021] Fix this math.  The interval should be
            Interval =
                Observable
                    .Interval(refreshInterval, SchedulerProvider.BackgroundThread)
                    .Scan(TimeSpan.Zero, (acc, _) => acc + refreshInterval)
                    .TakeUntil(x => x <= timePerPartition);
        }
    }
}