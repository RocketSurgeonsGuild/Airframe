using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Rocket.Surgery.Airframe.Forms;
using Rocket.Surgery.Airframe.Timers;
using Xunit;

namespace Airframe.Tests.Timers
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
            var testScheduler = new TestScheduler();
            SchedulerProvider schedulerProvider = new SchedulerProviderFixture().WithTestScheduler(testScheduler);
            IncrementTimer sut = new IncrementTimerFixture().WithProvider(schedulerProvider);

            // When
            sut.Start(TimeSpan.FromMinutes(25));
            testScheduler.AdvanceByMs(InitialMilliseconds);

            // Then
            sut.IsRunning.Should().BeTrue();
        }

        [Fact]
        public void Should_Advance_When_Started()
        {
            // Given
            var testScheduler = new TestScheduler();
            var schedulerProvider = new SchedulerProviderFixture().WithTestScheduler(testScheduler);
            IncrementTimer sut = new IncrementTimerFixture().WithProvider(schedulerProvider);
            var timer = TimeSpan.Zero;

            sut.Subscribe(x =>
            {
                timer = x;
            });

            // When
            sut.Start();
            testScheduler.AdvanceByMs(InitialMilliseconds);
            testScheduler.AdvanceByMs(OneThousandMilliseconds);

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void Should_Resume_Where_Stopped()
        {
            // Given
            var testScheduler = new TestScheduler();
            var schedulerProvider = new SchedulerProviderFixture().WithTestScheduler(testScheduler);
            IncrementTimer sut = new IncrementTimerFixture().WithProvider(schedulerProvider);
            var timer = TimeSpan.Zero;
            sut.Start(TimeSpan.FromMinutes(1));

            sut.Subscribe(x =>
            {
                timer = x;
            });

            sut.Start();
            testScheduler.AdvanceByMs(InitialMilliseconds);
            sut.Stop();
            testScheduler.AdvanceByMs(OneThousandMilliseconds);

            // When
            sut.Start();

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Should_Resume_After_Stopped()
        {
            // Given
            var testScheduler = new TestScheduler();
            var schedulerProvider = new SchedulerProviderFixture().WithTestScheduler(testScheduler);
            IncrementTimer sut = new IncrementTimerFixture().WithProvider(schedulerProvider);
            var timer = TimeSpan.Zero;

            sut.Subscribe(x =>
            {
                timer = x;
            });

            sut.Start();
            testScheduler.AdvanceByMs(InitialMilliseconds);
            sut.Stop();
            testScheduler.AdvanceByMs(OneThousandMilliseconds);

            // When
            sut.Start();
            testScheduler.AdvanceByMs(OneThousandMilliseconds);

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(2));
        }
    }
}