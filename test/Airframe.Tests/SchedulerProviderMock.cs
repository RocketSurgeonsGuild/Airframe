using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using Rocket.Surgery.Airframe.Scheduling;

namespace Airframe.Tests
{
    internal class SchedulerProviderMock : ISchedulerProvider
    {
        public SchedulerProviderMock(IScheduler main = null, IScheduler background = null)
        {
            UserInterfaceThread = main ?? new TestScheduler();
            BackgroundThread = background ?? new TestScheduler();
        }

        public IScheduler UserInterfaceThread { get; }

        public IScheduler BackgroundThread { get; }
    }
}