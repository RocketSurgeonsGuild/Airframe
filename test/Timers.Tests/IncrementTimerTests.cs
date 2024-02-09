using Airframe.Testing;
using FluentAssertions;
using ReactiveUI.Testing;
using Rocket.Surgery.Airframe.Tests;
using System;
using Xunit;

namespace Rocket.Surgery.Airframe.Timers.Tests
{
    public class IncrementTimerTests : TestBase
    {
        private const int InitialMilliseconds = 1001;
        private const int OneThousandMilliseconds = 1000;

        [Fact]
        public void Should_Not_Be_Running_When_Constructed()
        {
            // Given, When
            IncrementTimer sut = new IncrementTimerFixture();

            // Then
            sut.IsRunning.Should().BeFalse();
        }

        [Fact]
        public void Should_Be_Running_When_Started()
        {
            // Given
            SchedulerProviderMock schedulerProvider = new SchedulerProviderFixture();
            IncrementTimer sut = new IncrementTimerFixture().WithProvider(schedulerProvider);

            // When
            sut.Start(TimeSpan.FromMinutes(25));
            schedulerProvider.UserInterfaceTestScheduler.AdvanceByMs(InitialMilliseconds);

            // Then
            sut.IsRunning.Should().BeTrue();
        }

        [Fact]
        public void Should_Advance_When_Started()
        {
            // Given
            SchedulerProviderMock schedulerProvider = new SchedulerProviderFixture();
            IncrementTimer sut = new IncrementTimerFixture().WithProvider(schedulerProvider);
            var timer = TimeSpan.Zero;

            sut.Subscribe(x =>
            {
                timer = x;
            });

            // When
            sut.Start();
            schedulerProvider.UserInterfaceTestScheduler.AdvanceByMs(InitialMilliseconds);
            schedulerProvider.UserInterfaceTestScheduler.AdvanceByMs(OneThousandMilliseconds);

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void Should_Resume_Where_Stopped()
        {
            // Given
            SchedulerProviderMock schedulerProvider = new SchedulerProviderFixture();
            IncrementTimer sut = new IncrementTimerFixture().WithProvider(schedulerProvider);
            var timer = TimeSpan.Zero;
            sut.Start(TimeSpan.FromMinutes(1));

            using var _ = sut.Subscribe(x => timer = x);

            sut.Start();
            schedulerProvider.UserInterfaceTestScheduler.AdvanceByMs(InitialMilliseconds);
            sut.Stop();
            schedulerProvider.UserInterfaceTestScheduler.AdvanceByMs(OneThousandMilliseconds);

            // When
            sut.Start();

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Should_Resume_After_Stopped()
        {
            // Given
            SchedulerProviderMock schedulerProvider = new SchedulerProviderFixture();
            IncrementTimer sut = new IncrementTimerFixture().WithProvider(schedulerProvider);
            var timer = TimeSpan.Zero;

            sut.Subscribe(x => timer = x);

            sut.Start();
            schedulerProvider.UserInterfaceTestScheduler.AdvanceByMs(InitialMilliseconds);
            sut.Stop();
            schedulerProvider.UserInterfaceTestScheduler.AdvanceByMs(OneThousandMilliseconds);

            // When
            sut.Start();
            schedulerProvider.UserInterfaceTestScheduler.AdvanceByMs(OneThousandMilliseconds);

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(2));
        }
    }
}