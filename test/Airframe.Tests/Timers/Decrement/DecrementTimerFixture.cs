using System.Reactive.Concurrency;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Timers.UnitTests.Decrement
{
    internal class DecrementTimerFixture : ITestFixtureBuilder
    {
        private IScheduler _scheduler;

        public DecrementTimerFixture()
        {
            _scheduler = CurrentThreadScheduler.Instance;
        }

        public DecrementTimerFixture WithScheduler(IScheduler scheduler) => this.With(ref _scheduler, scheduler);

        public static implicit operator DecrementTimer(DecrementTimerFixture fixture) => fixture.Build();

        private DecrementTimer Build() => new DecrementTimer(_scheduler);
    }
}