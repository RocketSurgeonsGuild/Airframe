using System;

namespace Rocket.Surgery.Airframe.Timers
{
    public interface ITimer
    {
        IObservable<TimerEvent> Changed { get; }
        void Start();
        void Stop();
    }
}