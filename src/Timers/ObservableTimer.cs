using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Represents a default implementation of <see cref="IObservableTimer"/>.
    /// </summary>
    public abstract class ObservableTimer : ReactiveObject, ITimer, IObservable<TimeSpan>, IDisposable
    {
        private readonly ISubject<TimerEvent> _timerEvents = new Subject<TimerEvent>();
        private readonly ObservableAsPropertyHelper<bool> _isRunning;
        private readonly IObservable<TimeSpan> _elapsed;
        private TimeSpan _resumeTime = TimeSpan.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableTimer"/> class.
        /// </summary>
        /// <param name="schedulerProvider">The scheduler provider.</param>
        protected ObservableTimer(ISchedulerProvider schedulerProvider)
        {
                _timerEvents
                   .Select(x => x is TimerStartEvent)
                .ToProperty(this, nameof(IsRunning), out _isRunning)
                .DisposeWith(Garbage);

                TimeStartEvents =
                    _timerEvents
                       .OfType<TimerStartEvent>()
                       .Select(x => x.Duration);

                _elapsed =
                    _timerEvents
                       .CombineLatest(TimeStartEvents, (timerEvent, duration) => (duration, isRunning: timerEvent is TimerStartEvent))
                       .Select(x => x.isRunning
                                ? Observable
                                   .Interval(TimeSpans.RefreshInterval, schedulerProvider.BackgroundThread)
                                   .Scan(x.duration, (duration, _) => Accumulator(duration))
                                   .TakeUntil(Elapsed)
                                   .Do(elapsed => _resumeTime = elapsed, () => _resumeTime = x.duration)
                                : Observable.Return(_resumeTime))
                       .Switch();
        }

        /// <inheritdoc />
        public bool IsRunning => _isRunning.Value;

        /// <summary>
        /// Gets the <see cref="TimerStartEvent"/> stream.
        /// </summary>
        protected IObservable<TimeSpan> TimeStartEvents { get; }

        /// <summary>
        /// Gets the garbage.
        /// </summary>
        protected CompositeDisposable Garbage { get; } = new CompositeDisposable();

        /// <inheritdoc />
        public void Start(TimeSpan timeSpan) => _timerEvents.OnNext(new TimerStartEvent(timeSpan));

        /// <inheritdoc />
        public void Start() => _timerEvents.OnNext(new TimerStartEvent(_resumeTime));

        /// <inheritdoc />
        public void Stop() => _timerEvents.OnNext(new TimerStopEvent());

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TimeSpan> observer) =>
            _elapsed.Where(x => x >= TimeSpan.Zero).Subscribe(observer);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Accumulates the provided time.
        /// </summary>
        /// <param name="accumulated">The accumulated value.</param>
        /// <returns>The new time.</returns>
        protected abstract TimeSpan TimeAccumulator(TimeSpan accumulated);

        /// <summary>
        /// Gets a value indicating whether the time has elapsed.
        /// </summary>
        /// <param name="elapsed">The elapsed time.</param>
        /// <returns>Whether the time has elapsed.</returns>
        protected abstract bool Elapse(TimeSpan elapsed);

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">A value indicating whether or not you are disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Garbage.Dispose();
            }
        }

        private TimeSpan Accumulator(TimeSpan accumulated) => TimeAccumulator(accumulated);

        private bool Elapsed(TimeSpan elapsed) => Elapse(elapsed);
    }
}