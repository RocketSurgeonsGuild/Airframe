using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    ///     A timer that partitions time base on a provided partition number..
    /// </summary>
    public class DivisibleTimer
    {
        private readonly IScheduler _scheduler;
        private readonly BehaviorSubject<bool> _pause = new BehaviorSubject<bool>(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisibleTimer"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        public DivisibleTimer(IScheduler scheduler)
        {
            _scheduler = scheduler;
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
        public void Start(long partition, TimeSpan duration)
        {
            var refreshInterval = TimeSpan.FromMilliseconds(1000);

            long ticks = Math.DivRem(duration.Ticks, partition, out var remainder);

            TimeSpan remainderSpan = TimeSpan.FromTicks(ticks);

            IntervalTime = remainderSpan;

            // TODO: [rlittlesii: January 11, 2021] Fix this math.  The interval should be
            Interval =
                Observable
                    .Interval(refreshInterval, _scheduler)
                    .Scan(TimeSpan.Zero, (acc, _) => acc + refreshInterval)
                    .TakeUntil(x => x <= remainderSpan);
        }

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        public void Pause()
        {
            _pause.OnNext(true);
        }

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public void Resume()
        {
            _pause.OnNext(false);
        }
    }
}