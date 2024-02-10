using Airframe.Testing;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.Airframe.Timers.Tests;

internal class DecrementTimerFixture : ITestFixtureBuilder
{
    public static implicit operator DecrementTimer(DecrementTimerFixture fixture) => fixture.Build();

    public DecrementTimerFixture WithProvider(ISchedulerProvider schedulerProvider) => this.With(ref _schedulerProvider, schedulerProvider);

    private DecrementTimer Build() => new DecrementTimer(_schedulerProvider);

    private ISchedulerProvider _schedulerProvider = new SchedulerProviderFixture().AsInterface();
}