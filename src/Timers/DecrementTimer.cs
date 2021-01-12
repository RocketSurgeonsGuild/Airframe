using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Core;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    ///     A timer that starts at a certain point and counts down.
    /// </summary>
    public class DecrementTimer : TimerBase
    {
        private ObservableAsPropertyHelper<bool> _isRunning;
        private TimeSpan _pausedTime = TimeSpan.Zero;
        private TimeSpan _resumeTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DecrementTimer" /> class.
        /// </summary>
        /// <param name="schedulerProvider">The scheduler.</param>
        public DecrementTimer(ISchedulerProvider schedulerProvider)
            : base(schedulerProvider)
        {
        }

        /// <summary>
        ///     Gets a value indicating whether the timer is running.
        /// </summary>
        public bool IsRunning => _isRunning.Value;

        /// <summary>
        ///     Sets a timer.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="startImmediately">Whether to start the timer immediately.</param>
        /// <returns>An observable of time.</returns>
        public IObservable<TimeSpan> Timer(TimeSpan startTime, bool startImmediately = true)
        {
            var refreshInterval = TimeSpan.FromMilliseconds(1000);
            var running = Running.AsObservable().StartWith(startImmediately);

            _isRunning = running.ToProperty(this, x => x.IsRunning).DisposeWith(TimesUp);
            _resumeTime = startTime;

            var timer =
                Observable
                    .Create<TimeSpan>(observer =>
                        Observable
                            .Interval(refreshInterval, BackgroundThread)
                            .Scan(_resumeTime, (acc, value) => acc - refreshInterval)
                            .TakeUntil(x => x <= TimeSpan.FromSeconds(0))
                            .Do(x => _resumeTime = x, () => _resumeTime = startTime)
                            .Subscribe(observer)
                            .DisposeWith(TimesUp));

            return
                running
                    .Select(isRunning => isRunning ? timer : Observable.Never<TimeSpan>())
                    .Switch()
                    .Publish()
                    .RefCount();
        }

        /// <summary>
        ///     Start the timer.
        /// </summary>
        /// <returns>A notification.</returns>
        public IObservable<Unit> Start()
        {
            Running.OnNext(true);
            return Observable.Return(Unit.Default);
        }

        /// <summary>
        ///     Pause the timer.
        /// </summary>
        /// <returns>A notification.</returns>
        public IObservable<Unit> Pause()
        {
            Running.OnNext(false);
            return Observable.Return(Unit.Default);
        }

        /// <summary>
        ///     Resume the timer.
        /// </summary>
        /// <returns>A notification.</returns>
        public IObservable<Unit> Resume()
        {
            Running.OnNext(true);
            return Observable.Return(Unit.Default);
        }
    }
}