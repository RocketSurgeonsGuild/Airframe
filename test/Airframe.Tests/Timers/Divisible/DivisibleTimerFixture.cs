using ReactiveUI.Testing;
using Rocket.Surgery.Airframe.Forms;
using Rocket.Surgery.Airframe.Timers;

namespace Airframe.Tests.Timers
{
    internal class DivisibleTimerFixture : IBuilder
    {
        private SchedulerProvider _schedulerProvider = new SchedulerProviderFixture();

        public static implicit operator DivisibleTimer(DivisibleTimerFixture fixture) => fixture.Build();

        public DivisibleTimerFixture WithProvider(SchedulerProvider scheduler) => this.With(ref _schedulerProvider, scheduler);

        private DivisibleTimer Build() => new DivisibleTimer(_schedulerProvider);
    }
}