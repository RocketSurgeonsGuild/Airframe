using System;
using FluentAssertions;
using Rocket.Surgery.Airframe.Timers;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using Xunit;

namespace Airframe.Tests.Timers
{
    public class TimerTests
    {
        [Fact]
        public void Should()
        {
            // Given
            TimeSpan result = TimeSpan.Zero;
            Timer sut = new TimerFixture();

            // When
            sut.Subscribe(x => result = x);

            // Then
            result
                .Should()
                .NotBe(TimeSpan.Zero);
        }
    }

    internal class TimerFixture : ITestFixtureBuilder
    {
        public static implicit operator Timer(TimerFixture fixture) => fixture.Build();

        private Timer Build() => new Timer(new SchedulerProviderMock());
    }
}