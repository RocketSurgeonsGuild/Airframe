using Airframe.Testing;
using FluentAssertions;
using System;
using System.Reactive.Linq;
using Xunit;

namespace Rocket.Surgery.Airframe.Timers.Tests;

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
        SchedulerProviderMock schedulerProvider = new SchedulerProviderFixture();
        DivisibleTimer sut =
            new DivisibleTimerFixture().WithProvider(schedulerProvider);
        sut.Interval.ObserveOn(schedulerProvider.UserInterfaceTestScheduler).Subscribe(_ => count++);

        // When
        sut.Start(4, TimeSpan.FromHours(1));
        schedulerProvider.UserInterfaceTestScheduler.AdvanceBy(TimeSpan.FromMinutes(30).Ticks);

        // Then
        count.Should().Be(4);
    }
}