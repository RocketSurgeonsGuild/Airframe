using System;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Represents an <see cref="ITimer"/> that is observable.
    /// </summary>
    public class ObservableTimer
    {
        private Subject<TimeSpan> _timer = new Subject<TimeSpan>();

        /// <inheritdoc/>
        public bool IsRunning { get; }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TimeSpan> observer)
        {
            return _timer.Subscribe(observer);
        }

        /// <inheritdoc/>
        public void Start()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}