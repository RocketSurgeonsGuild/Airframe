using System;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Interface representing a base timer.
    /// </summary>
    public interface ITimer : IObservable<TimeSpan>
    {
        bool IsRunning { get; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        void Stop();
    }

    public class ObservableTimer : ITimer
    {
        private Subject<TimeSpan> _timer = new Subject<TimeSpan>();

        public IDisposable Subscribe(IObserver<TimeSpan> observer)
        {
            return _timer.Subscribe(observer);
        }

        public bool IsRunning { get; }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}