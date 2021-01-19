using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveMarbles.PropertyChanged;
using ReactiveUI;
using Rocket.Surgery.Airframe.Scheduling;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Represents a default implementation of <see cref="IObservableTimer"/>.
    /// </summary>
    public class ObservableTimer : ReactiveObject, IObservableTimer, IDisposable
    {
        private readonly ISubject<TimerEvent> _timerEvents = new Subject<TimerEvent>();
        private readonly ObservableAsPropertyHelper<bool> _isRunning;
        private readonly IObservable<TimeSpan> _elapsed;
        private TimeSpan _duration;
        private TimeSpan _resumeTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableTimer"/> class.
        /// </summary>
        /// <param name="schedulerProvider">The scheduler provider.</param>
        public ObservableTimer(ISchedulerProvider schedulerProvider)
        {
            _timerEvents
                .Select(x => x is TimerStartEvent)
                .ToProperty(this, nameof(IsRunning), out _isRunning)
                .DisposeWith(Subscriptions);

            this.WhenPropertyValueChanges(x => x.Duration)
                .Where(x => x > TimeSpan.Zero)
                .Take(1)
                .Subscribe(x => _resumeTime = x)
                .DisposeWith(Subscriptions);

            _elapsed =
                _timerEvents
                    .CombineLatest(
                        this.WhenPropertyValueChanges(x => x.Duration).Where(x => x != TimeSpan.Zero),
                        this.WhenPropertyValueChanges(x => x.IsRunning),
                        (timerEvent, duration, isRunning) => (timerEvent, duration, isRunning))
                    .Select(x =>
                    {
                        return x.isRunning
                            ? Observable
                                .Interval(TimeSpans.RefreshInterval, schedulerProvider.BackgroundThread)
                                .Scan(_resumeTime, (acc, _) => acc - TimeSpans.RefreshInterval)
                                .TakeUntil(elapsed => elapsed <= TimeSpan.Zero)
                                .Do(elapsed => _resumeTime = elapsed, () => _resumeTime = x.duration)
                            : Observable.Never<TimeSpan>();
                    })
                    .Switch();
        }

        /// <inheritdoc />
        public bool IsRunning => _isRunning.Value;

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public TimeSpan Duration
        {
            get => _duration;
            private set => this.RaiseAndSetIfChanged(ref _duration, value);
        }

        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        protected CompositeDisposable Subscriptions { get; } = new CompositeDisposable();

        /// <inheritdoc />
        public void Start() => _timerEvents.OnNext(new TimerStartEvent());

        /// <inheritdoc />
        public void Stop() => _timerEvents.OnNext(new TimerStopEvent());

        /// <inheritdoc />
        public void Set(TimeSpan timeSpan)
        {
            Duration = timeSpan;
        }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TimeSpan> observer) => _elapsed.Where(x => x >= TimeSpan.Zero).Subscribe(observer);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposes of resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether it is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Subscriptions.Dispose();
            }
        }
    }
}