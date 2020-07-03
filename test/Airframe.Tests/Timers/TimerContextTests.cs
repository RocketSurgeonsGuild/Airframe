using ReactiveUI.Testing;
using Rocket.Surgery.Airframe.Timers;

namespace Airframe.Tests.Timers
{
    public class TimerContextTests
    {
        
    }

    internal class TimerContextFixture : IBuilder
    {
        public static implicit operator TimerContext(TimerContextFixture fixture) => fixture.Build();

        private TimerContext Build() => new TimerContext();
    }
}