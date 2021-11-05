using System;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Interface representing an incrementing timer.
    /// </summary>
    public interface IIncrement : ITimer, IObservable<TimeSpan>
    {
    }
}
