using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Rocket.Surgery.Extensions.Testing;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Timers.UnitTests
{
    public class RoundTimerTests : AutoSubstituteTest
    {
        public RoundTimerTests(ITestOutputHelper outputHelper)
            : base(outputHelper) { }

        [Fact]
        public void Test1()
        {
            // Given
            var interval = TimeSpan.Zero;
            var testScheduler = new TestScheduler();
            RoundTimer  sut = new RoundTimer(testScheduler);
            sut.Interval.ObserveOn(testScheduler).Subscribe(_ => interval = _);

            // When
            sut.SetTimer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1)).Subscribe();
            testScheduler.AdvanceByMs(3001);

            // Then
            interval.Ticks.Should().Be(2);
        }
    }
}