using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Timers
{
    public class DivisibleTimer
    {
        private readonly BehaviorSubject<bool> _pause = new BehaviorSubject<bool>(false);

        public IObservable<TimeSpan> Interval { get; set; }

        public void Start(double exercises, TimeSpan duration)
        {
            var intervalDuration = duration.TotalSeconds / exercises;
            var timespan = TimeSpan.FromSeconds(intervalDuration);

            Interval = Observable.Interval(timespan).Select(x => TimeSpan.FromSeconds(x));
        }

        public void Pause()
        {
            _pause.OnNext(true);
        }

        public void Resume()
        {
            _pause.OnNext(false);
        }
    }
}