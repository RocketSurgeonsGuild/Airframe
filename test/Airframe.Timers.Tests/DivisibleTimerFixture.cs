using Airframe.Tests;
using Rocket.Surgery.Airframe.Forms;
using Rocket.Surgery.Airframe.Timers;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Airframe.Timers.Tests
{
    internal class DivisibleTimerFixture : ITestFixtureBuilder
    {
        private SchedulerProvider _schedulerProvider = new SchedulerProviderFixture();

        public static implicit operator DivisibleTimer(DivisibleTimerFixture fixture) => fixture.Build();

        public DivisibleTimerFixture WithProvider(SchedulerProvider scheduler) => this.With(ref _schedulerProvider, scheduler);

        private DivisibleTimer Build() => new DivisibleTimer(_schedulerProvider);
    }
}