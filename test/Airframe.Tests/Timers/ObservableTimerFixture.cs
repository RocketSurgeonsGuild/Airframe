using Rocket.Surgery.Airframe.Forms;
using Rocket.Surgery.Airframe.Timers;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Airframe.Tests.Timers
{
    internal class ObservableTimerFixture : ITestFixtureBuilder
    {
        private SchedulerProvider _schedulerProvider = new SchedulerProviderFixture();

        public static implicit operator ObservableTimer(ObservableTimerFixture fixture) => fixture.Build();

        public ObservableTimerFixture WithProvider(SchedulerProvider schedulerProvider) =>
            this.With(ref _schedulerProvider, schedulerProvider);

        private ObservableTimer Build() => new ObservableTimer(_schedulerProvider);
    }
}