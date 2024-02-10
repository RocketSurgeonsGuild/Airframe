using Microsoft.Reactive.Testing;
using Rocket.Surgery.Airframe;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;

namespace Airframe.Testing;

[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order")]
public class SchedulerProviderFixture : ITestFixtureBuilder
{
    public SchedulerProviderFixture() => _mainThreadScheduler = _backgroundThreadScheduler = new TestScheduler();

    public static implicit operator SchedulerProviderMock(SchedulerProviderFixture fixture) => fixture.Build();

    public SchedulerProviderFixture WithUserInterfaceScheduler(IScheduler scheduler) => this.With(ref _mainThreadScheduler, scheduler);

    public SchedulerProviderFixture WithBackgroundScheduler(IScheduler scheduler) => this.With(ref _backgroundThreadScheduler, scheduler);

    public SchedulerProviderFixture WithTestScheduler(IScheduler scheduler) => WithUserInterfaceScheduler(scheduler).WithBackgroundScheduler(scheduler);

    public ISchedulerProvider AsInterface() => Build();

    private SchedulerProviderMock Build() => new SchedulerProviderMock(_mainThreadScheduler, _backgroundThreadScheduler);

    private IScheduler _mainThreadScheduler;
    private IScheduler _backgroundThreadScheduler;
}