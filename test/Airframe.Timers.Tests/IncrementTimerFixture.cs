using Airframe.Tests;
using Rocket.Surgery.Airframe.Forms;
using Rocket.Surgery.Airframe.Timers;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Airframe.Timers.Tests
{
    internal class IncrementTimerFixture : ITestFixtureBuilder
    {
        private SchedulerProvider _schedulerProvider = new SchedulerProviderFixture();

        public static implicit operator IncrementTimer(IncrementTimerFixture fixture) => fixture.Build();

        public IncrementTimerFixture WithProvider(SchedulerProvider schedulerProvider) =>
            this.With(ref _schedulerProvider, schedulerProvider);

        private IncrementTimer Build() => new IncrementTimer(_schedulerProvider);
    }
}