using Airframe.Testing;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.Airframe.Timers.Tests
{
    internal class DivisibleTimerFixture : ITestFixtureBuilder
    {
        private ISchedulerProvider _schedulerProvider = new SchedulerProviderFixture().AsInterface();

        public static implicit operator DivisibleTimer(DivisibleTimerFixture fixture) => fixture.Build();

        public DivisibleTimerFixture WithProvider(ISchedulerProvider scheduler) => this.With(ref _schedulerProvider, scheduler);

        private DivisibleTimer Build() => new DivisibleTimer(_schedulerProvider);
    }
}