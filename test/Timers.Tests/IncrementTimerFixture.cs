using Airframe.Testing;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.Airframe.Timers.Tests;

internal class IncrementTimerFixture : ITestFixtureBuilder
{
    public static implicit operator IncrementTimer(IncrementTimerFixture fixture) => fixture.Build();

    public IncrementTimerFixture WithProvider(ISchedulerProvider schedulerProvider) => this.With(ref _schedulerProvider, schedulerProvider);

    private IncrementTimer Build() => new IncrementTimer(_schedulerProvider);

    private ISchedulerProvider _schedulerProvider = new SchedulerProviderFixture().AsInterface();
}