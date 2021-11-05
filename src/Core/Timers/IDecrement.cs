using System;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Interface representing an decrementing timer.
    /// </summary>
    public interface IDecrement : ITimer, IObservable<TimeSpan>
    {
    }
}
