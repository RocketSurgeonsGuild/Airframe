using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Represents the internal timer context.
    /// </summary>
    public class TimerContext : ReactiveObject
    {
        private readonly BehaviorSubject<bool> _cancelRequested;
        private readonly Subject<TimeSpan> _progressDeltas;
        private readonly ObservableAsPropertyHelper<bool> _isCancelled;
        private ObservableAsPropertyHelper<TimeSpan> _progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerContext"/> class.
        /// </summary>
        public TimerContext()
        {
            _cancelRequested = new BehaviorSubject<bool>(false);
            _progressDeltas = new Subject<TimeSpan>();

            _isCancelled =
                _cancelRequested
                    .ToProperty(this, x => x.IsCancelled);

            _progress =
                _progressDeltas
                    .Scan((running, next) => running + next)
                    .ToProperty(this, x => x.Progress);
        }

        /// <summary>
        /// Gets the remaining time for the timer.
        /// </summary>
        public TimeSpan Progress { get; }

        /// <summary>
        /// Gets a value indicating whether or not the timer is cancelled.
        /// </summary>
        public bool IsCancelled { get; }

        /// <summary>
        /// Gets a value indicating whether or not the timer is running.
        /// </summary>
        public bool IsRunning { get; }
    }
}