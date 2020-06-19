using System;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Timers
{
    public class DivisibleTimer
    {
        readonly BehaviorSubject<bool> _pause = new BehaviorSubject<bool>(false);

        public void Start(int exercises, TimeSpan duration)
        {
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