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
    public sealed class ObservableTimer : ReactiveObject, IObservableTimer, IDisposable
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
            var start = _timerEvents
               .Select(x => x is TimerStartEvent);
            start
                .ToProperty(this, nameof(IsRunning), out _isRunning)
                .DisposeWith(_subscriptions);

            _elapsed =
                _timerEvents
                    .CombineLatest(
                        _timerEvents
                           .Where(x => x is TimerStartEvent)
                           .Cast<TimerStartEvent>()
                           .Select(x => x.Duration),
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
        public void Dispose() => _subscriptions.Dispose();
    }
}