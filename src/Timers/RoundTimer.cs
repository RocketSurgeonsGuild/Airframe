using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// A timer that defines a round and a rest period.
    /// </summary>
    public class RoundTimer
    {
        private readonly IScheduler _scheduler;
        private readonly Subject<(TimeSpan, TimeSpan)> _observerSubject = new Subject<(TimeSpan, TimeSpan)>();

        private readonly IObservable<IObservable<Unit>> _whatever =
            Observable.Create<IObservable<Unit>>(observer => Disposable.Empty);

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundTimer"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        public RoundTimer(IScheduler scheduler = null)
        {
            _scheduler = scheduler ?? CurrentThreadScheduler.Instance;

            // TODO: [rlittlesii: June 13, 2020] Set interval for the workout
            // TODO: [rlittlesii: June 13, 2020] Set interval for rest
            // TODO: [rlittlesii: June 13, 2020] Set repeat amount
            // TODO: [rlittlesii: June 13, 2020] Pause
            // TODO: [rlittlesii: June 13, 2020] Reset
            _observerSubject
                .AsObservable()
                .Select(x =>
                    Observable.Create<Unit>(_ =>
                    {
                        On = Observable.Interval(x.Item1, _scheduler).Select(TimeSpan.FromTicks);
                        Off = Observable.Interval(x.Item2, _scheduler).Select(TimeSpan.FromTicks);
                        return Disposable.Empty;
                    }));

            On = Observable.Create<TimeSpan>(observer => Disposable.Empty);
            Off = Observable.Create<TimeSpan>(observer => Disposable.Empty);
        }

        /// <inheritdoc />
        public IObservable<TimeSpan> On { get; private set; }

        /// <inheritdoc />
        public IObservable<TimeSpan> Off { get; private set; }

        /// <inheritdoc />
        public IObservable<Unit> SetTimer(TimeSpan workoutInterval, TimeSpan restInterval)
        {
            return Observable.Create<Unit>(observer =>
            {
                _whatever
                    .Take(1)
                    .Merge()
                    .ObserveOn(_scheduler)
                    .Subscribe(observer);

                _observerSubject
                    .OnNext((workoutInterval, restInterval));

                return Disposable.Empty;
            });
        }
    }
}