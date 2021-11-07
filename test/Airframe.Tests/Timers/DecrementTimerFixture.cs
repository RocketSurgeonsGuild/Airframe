using Rocket.Surgery.Airframe.Forms;
using Rocket.Surgery.Airframe.Timers;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Airframe.Tests.Timers
{
    internal class DecrementTimerFixture : ITestFixtureBuilder
    {
        private SchedulerProvider _schedulerProvider = new SchedulerProviderFixture();

        public static implicit operator DecrementTimer(DecrementTimerFixture fixture) => fixture.Build();

        public DecrementTimerFixture WithProvider(SchedulerProvider schedulerProvider) =>
            this.With(ref _schedulerProvider, schedulerProvider);

        private DecrementTimer Build() => new DecrementTimer(_schedulerProvider);
    }
}