using System.Reactive.Concurrency;
using Core;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Rocket.Surgery.Airframe.Timers;

namespace Airframe.Tests.Timers.Divisible
{
    internal class DivisibleTimerFixture : IBuilder
    {
        private IScheduler _scheduler;
        private ISchedulerProvider _schedulerProvider;

        public DivisibleTimerFixture()
        {
            _scheduler = new TestScheduler();
        }

        public static implicit operator DivisibleTimer(DivisibleTimerFixture fixture) => fixture.Build();

        public DivisibleTimerFixture WithScheduler(IScheduler scheduler) => this.With(ref _scheduler, scheduler);
        public DivisibleTimerFixture WithProvider(ISchedulerProvider scheduler) => this.With(ref _schedulerProvider, scheduler);

        private DivisibleTimer Build() => new DivisibleTimer(_scheduler);
    }
}