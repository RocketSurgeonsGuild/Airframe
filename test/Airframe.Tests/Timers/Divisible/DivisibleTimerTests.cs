using Rocket.Surgery.Airframe.Timers;

namespace Airframe.Tests.Timers.Divisible
{
    public class DivisibleTimerTests
    {
        
    }

    internal class DivisibleTimerFixture
    {
        public static implicit operator DivisibleTimer(DivisibleTimerFixture fixture) => fixture.Build();

        private DivisibleTimer Build() => new DivisibleTimer();
    }
}