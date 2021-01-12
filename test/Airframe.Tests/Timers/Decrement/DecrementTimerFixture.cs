using System.Reactive.Concurrency;
using Core;
using Microsoft.Reactive.Testing;
using Rocket.Surgery.Airframe.Timers;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Airframe.Tests.Timers
{
    internal class DecrementTimerFixture : ITestFixtureBuilder
    {
        private IScheduler _scheduler;
        private ISchedulerProvider _schedulerProvider = new SchedulerProviderMock();

        public DecrementTimerFixture()
        {
            _scheduler = CurrentThreadScheduler.Instance;
        }

        public DecrementTimerFixture WithScheduler(IScheduler scheduler) => this.With(ref _scheduler, scheduler);
        public DecrementTimerFixture WithProvider(ISchedulerProvider scheduler) => this.With(ref _schedulerProvider, scheduler);

        public static implicit operator DecrementTimer(DecrementTimerFixture fixture) => fixture.Build();

        private DecrementTimer Build() => new DecrementTimer(_schedulerProvider);
    }

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