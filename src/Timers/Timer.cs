using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Core;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.Timers
{
    public class Timer : ReactiveObject, ITimer, IDisposable
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly ISubject<TimerEvent> _timerEvents = new Subject<TimerEvent>();
        private ObservableAsPropertyHelper<bool> _isRunning;
        private TimeSpan _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        /// <param name="schedulerProvider">The scheduler provider.</param>
        public Timer(ISchedulerProvider schedulerProvider)
        {
            _schedulerProvider = schedulerProvider;

            _timerEvents
                .Select(x => x is TimerStartEvent || x is TimerResumeEvent)
                .ToProperty(this, nameof(IsRunning), out _isRunning)
                .DisposeWith(Subscriptions);

            this.WhenAnyValue(x => x.Duration)
                .CombineLatest(_timerEvents, (duration, timerEvent) => (duration, timerEvent))
                .Where(x => x.timerEvent is TimerStartEvent || x.timerEvent is TimerResumeEvent)
                .Select(x => x.duration)
                .Subscribe()
                .DisposeWith(Subscriptions);
        }

        /// <inheritdoc />
        public bool IsRunning => _isRunning.Value;

        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        protected CompositeDisposable Subscriptions { get; } = new();

        private TimeSpan Duration
        {
            get => _duration;
            set => this.RaiseAndSetIfChanged(ref _duration, value);
        }

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
        public IDisposable Subscribe(IObserver<TimeSpan> observer) => Disposable.Empty;

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