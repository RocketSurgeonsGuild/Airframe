using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Timers
{
    public class RoundTimer
    {
        private Subject<(TimeSpan, TimeSpan)> observerSubject = new Subject<(TimeSpan, TimeSpan)>();
        private IObservable<IObservable<Unit>> whatever =
            Observable.Create<IObservable<Unit>>(observer =>
            {
                return Disposable.Empty;
            });
        private IScheduler _scheduler;

        public RoundTimer(IScheduler scheduler = null)
        {
            _scheduler = scheduler ?? CurrentThreadScheduler.Instance;

            // TODO: [rlittlesii: June 13, 2020] Set interval for the workout
            // TODO: [rlittlesii: June 13, 2020] Set interval for rest
            // TODO: [rlittlesii: June 13, 2020] Set repeat amount
            // TODO: [rlittlesii: June 13, 2020] Pause
            // TODO: [rlittlesii: June 13, 2020] Reset

            observerSubject
                .AsObservable()
                .Select(x =>
                    Observable.Create<Unit>(_ =>
                    {
                        Interval = Observable.Interval(x.Item1, _scheduler).Select(TimeSpan.FromTicks);
                        Rest = Observable.Interval(x.Item2, _scheduler).Select(TimeSpan.FromTicks);
                        return Disposable.Empty;
                    }));
            Interval = Observable.Create<TimeSpan>(observer => Disposable.Empty);
            Rest = Observable.Create<TimeSpan>(observer => Disposable.Empty);
        }

        public IObservable<TimeSpan> Interval { get; private set; }

        public IObservable<TimeSpan> Rest { get; private set; }

        public IObservable<Unit> SetTimer(TimeSpan workoutInterval, TimeSpan restInterval)
        {
            return Observable.Create<Unit>(observer =>
            {
                whatever
                    .Take(1)
                    .Merge()
                    .ObserveOn(_scheduler)
                    .Subscribe(observer);

                observerSubject
                    .OnNext((workoutInterval, restInterval));

                return Disposable.Empty;
            });
        }
    }
}