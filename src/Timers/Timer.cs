using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Core;
using ReactiveMarbles.PropertyChanged;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Represents a default implementation of <see cref="ITimer"/>.
    /// </summary>
    public class Timer : ReactiveObject, ITimer, IDisposable
    {
        private readonly ISubject<TimerEvent> _timerEvents = new Subject<TimerEvent>();
        private ObservableAsPropertyHelper<bool> _isRunning;
        private TimeSpan _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        /// <param name="schedulerProvider">The scheduler provider.</param>
        public Timer(ISchedulerProvider schedulerProvider)
        {
            _timerEvents
                .Select(x => x is TimerStartEvent || x is TimerResumeEvent)
                .ToProperty(this, nameof(IsRunning), out _isRunning)
                .DisposeWith(Subscriptions);

            Elapsed =
                _timerEvents
                    .Where(x => x is TimerStartEvent || x is TimerResumeEvent)
                    .CombineLatest(
                        this.WhenPropertyValueChanges(x => x.Duration).Where(x => x != TimeSpan.Zero),
                        (timerEvent, duration) => (timerEvent, duration))
                    .Select(x =>
                    {
                        var resumeTime = x.duration;
                        return Observable
                            .Interval(TimeSpans.RefreshInterval, schedulerProvider.BackgroundThread)
                            .SubscribeOn(schedulerProvider.BackgroundThread)
                            .Scan(resumeTime, (acc, _) => acc - TimeSpans.RefreshInterval)
                            .TakeUntil(elapsed => elapsed <= TimeSpan.Zero)
                            .Do(elapsed => resumeTime = elapsed, () => resumeTime = x.duration);
                    })
                    .Switch();
        }

        /// <summary>
        /// Gets the elapsed time as an observable.
        /// </summary>
        public IObservable<TimeSpan> Elapsed { get; }

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
        protected CompositeDisposable Subscriptions { get; } = new();

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
        public IDisposable Subscribe(IObserver<TimeSpan> observer) => Elapsed.Where(x => x != TimeSpan.Zero).Subscribe(observer);

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