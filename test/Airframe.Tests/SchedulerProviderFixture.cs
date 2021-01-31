using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using Rocket.Surgery.Airframe.Forms;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Airframe.Tests
{
    internal class SchedulerProviderFixture : ITestFixtureBuilder
    {
        private IScheduler _mainThreadScheduler = new TestScheduler();
        private IScheduler _backgroundThreadScheduler = new TestScheduler();

        public SchedulerProviderFixture WithTestScheduler(IScheduler scheduler)
            => this.With(ref _mainThreadScheduler, scheduler)
               .With(ref _backgroundThreadScheduler, scheduler);

        public static implicit operator SchedulerProvider(SchedulerProviderFixture fixture) => fixture.Build();

        private SchedulerProvider Build() => new SchedulerProvider(_mainThreadScheduler, _backgroundThreadScheduler);
    }
}