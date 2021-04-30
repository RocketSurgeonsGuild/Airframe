using System;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Rocket.Surgery.Airframe.Timers;
using Xunit;

namespace Airframe.Tests.Timers
{
    public sealed class DivisibleTimerTests
    {
        [Theory]
        [ClassData(typeof(DivisibleTimeSpanData))]
        public void Should_Divide_TimeSpan_By_Partition(int partition, TimeSpan duration)
        {
            // Given
            DivisibleTimer sut = new DivisibleTimerFixture();

            // When
            sut.Start(partition, duration);

            // Then
            sut.IntervalTime.Should().Be(TimeSpan.FromTicks(Math.DivRem(duration.Ticks, partition, out var remainder)));
        }

        [Fact(Skip = "DivisibleTimer implementation not working correctly.")]
        public void Should_Run_Correct_Partition_Count()
        {
            // Given
            int count = 1;
            var testScheduler = new TestScheduler();
            DivisibleTimer sut =
                new DivisibleTimerFixture().WithProvider(new SchedulerProviderFixture().WithTestScheduler(testScheduler));
            sut.Interval.ObserveOn(testScheduler).Subscribe(_ => count++);

            // When
            sut.Start(4, TimeSpan.FromHours(1));
            testScheduler.AdvanceBy(TimeSpan.FromMinutes(30).Ticks);

            // Then
            count.Should().Be(4);
        }
    }
}