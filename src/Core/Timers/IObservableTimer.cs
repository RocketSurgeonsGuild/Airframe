using System;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Represents an <see cref="ITimer"/> that is observable.
    /// </summary>
    public interface IObservableTimer : ITimer, IObservable<TimeSpan>
    {
    }
}