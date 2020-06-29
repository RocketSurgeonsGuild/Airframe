using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Rocket.Surgery.Airframe.Timers;
using Xunit;

namespace Airframe.Tests.Timers.Divisible
{
    public sealed class DivisibleTimerTests
    {
        [Theory]
        [ClassData(typeof(DivisibleTimeSpanData))]
        public void Should_Divide_TimeSpan_By_Partition(long partition, TimeSpan duration)
        {
            // Given
            DivisibleTimer sut = new DivisibleTimerFixture();

            // When
            sut.Start(partition, duration);

            // Then
            sut.IntervalTime.Should().Be(TimeSpan.FromSeconds(duration.Ticks/partition));
        }

        [Fact]
        public void Should_Run_Correct_Partition_Count()
        {
            // Given
            int count = 1;
            var testScheduler = new TestScheduler();
            DivisibleTimer sut = new DivisibleTimerFixture().WithScheduler(testScheduler);
            sut.Interval.Subscribe(_ => count++);

            // When
            sut.Start(4, TimeSpan.FromHours(1));
            testScheduler.AdvanceBy(TimeSpan.FromHours(1).Ticks + 1);

            // Then
            count.Should().Be(4);
        }
    }
}