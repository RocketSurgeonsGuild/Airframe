using Rocket.Surgery.Airframe.Scheduling;
using Rocket.Surgery.Airframe.Timers;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Airframe.Tests.Timers
{
    internal class ObservableTimerFixture : ITestFixtureBuilder
    {
        private ISchedulerProvider _schedulerProvider = new SchedulerProviderMock();

        public static implicit operator ObservableTimer(ObservableTimerFixture fixture) => fixture.Build();

        public ObservableTimerFixture WithProvider(ISchedulerProvider schedulerProvider) =>
            this.With(ref _schedulerProvider, schedulerProvider);

        private ObservableTimer Build() => new ObservableTimer(_schedulerProvider);
    }
}