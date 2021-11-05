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
    public class ObservableTimer : ReactiveObject, IIncrement, IDecrement, IDisposable
    {
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
        private readonly ISubject<TimerEvent> _timerEvents = new Subject<TimerEvent>();
        private readonly ObservableAsPropertyHelper<bool> _isRunning;
        private readonly IObservable<TimeSpan> _elapsed;
        private TimeSpan _resumeTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableTimer"/> class.
        /// </summary>
        /// <param name="schedulerProvider">The scheduler provider.</param>
        public ObservableTimer(ISchedulerProvider schedulerProvider)
        {
            var start =
                _timerEvents
                   .Select(x => x is TimerStartEvent);

            // TODO: [rlittlesii: November 05, 2021] note
            start
                .ToProperty(this, nameof(IsRunning), out _isRunning)
                .DisposeWith(_subscriptions);

            TimeStartEvents =
                _timerEvents
                   .OfType<TimerStartEvent>()
                   .Select(x => x.Duration);

            _elapsed =
                _timerEvents
                    .CombineLatest(
                        TimeStartEvents,
                        start,
                        (_, duration, isRunning) => (duration, isRunning))
                    .Select(x => x.isRunning
                        ? Observable
                            .Interval(TimeSpans.RefreshInterval, schedulerProvider.BackgroundThread)
                            .Scan(x.duration, (acc, _) => acc - TimeSpans.RefreshInterval)
                            .TakeUntil(elapsed => elapsed <= TimeSpan.Zero)
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

        /// <inheritdoc />
        public void Start(TimeSpan timeSpan) => _timerEvents.OnNext(new TimerStartEvent(timeSpan));

        /// <inheritdoc />
        public void Start() => _timerEvents.OnNext(new TimerStartEvent(_resumeTime));

        /// <inheritdoc />
        public void Stop() => _timerEvents.OnNext(new TimerStopEvent());

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TimeSpan> observer) =>
            _elapsed.Where(x => x >= TimeSpan.Zero).Subscribe(observer);

        /// <inheritdoc/>
        IDisposable IIncrement.Subscribe(IObserver<TimeSpan> observer) =>
            Observable.Interval(TimeSpan.Zero)
               .Scan(TimeSpan.Zero, (acc, _) => acc + TimeSpans.RefreshInterval).Subscribe(observer);

        /// <inheritdoc/>
        IDisposable IDecrement.Subscribe(IObserver<TimeSpan> observer) =>
            _elapsed.Where(x => x >= TimeSpan.Zero).Subscribe(observer);

        /// <inheritdoc />
        public void Dispose() => _subscriptions.Dispose();
    }
}