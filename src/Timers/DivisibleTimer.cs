using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    ///     A timer that partitions time base on a provided partition number..
    /// </summary>
    public class DivisibleTimer
    {
        private readonly BehaviorSubject<bool> _pause = new BehaviorSubject<bool>(false);

        /// <summary>
        /// Gets the interval timer.
        /// </summary>
        public IObservable<TimeSpan> Interval { get; private set; }

        /// <summary>
        /// Gets the over all timer.
        /// </summary>
        public IObservable<TimeSpan> Timer { get; private set; }

        /// <summary>
        /// Started the timer.
        /// </summary>
        /// <param name="partition">The partition.</param>
        /// <param name="duration">The overall duration.</param>
        public void Start(double partition, TimeSpan duration)
        {
            var intervalDuration = duration.TotalSeconds / partition;
            var timespan = TimeSpan.FromSeconds(intervalDuration);

            Interval = Observable.Interval(timespan).Select(x => TimeSpan.FromSeconds(x));
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