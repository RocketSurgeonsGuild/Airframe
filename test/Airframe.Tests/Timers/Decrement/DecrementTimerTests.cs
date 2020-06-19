using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace Timers.UnitTests.Decrement
{
    public sealed class DecrementTimerTests
    {
        [Fact]
        public void Should_Take()
        {
            // Given
            var testScheduler = new TestScheduler();
            DecrementTimer sut = new DecrementTimerFixture().WithScheduler(testScheduler);
            var timer = TimeSpan.Zero;

            // When
            sut
                .Timer(TimeSpan.FromSeconds(3))
                .Subscribe(x =>
                {
                    timer = x;
                });

            testScheduler.AdvanceByMs(4000);

            // Then
            timer.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void Should_Take_Until_Pause()
        {
            // Given
            var testScheduler = new TestScheduler();
            DecrementTimer sut = new DecrementTimerFixture().WithScheduler(testScheduler);
            var timer = TimeSpan.Zero;

            // When
            sut
                .Timer(TimeSpan.FromSeconds(3))
                .Subscribe(x =>
                {
                    timer = x;
                });

            testScheduler.AdvanceByMs(1001);
            timer.Should().Be(TimeSpan.FromSeconds(2));
            sut.Pause().Subscribe();
            testScheduler.AdvanceByMs(10000);

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void Should_Take_Until_Zero()
        {
            // Given
            var testScheduler = new TestScheduler();
            DecrementTimer sut = new DecrementTimerFixture().WithScheduler(testScheduler);
            var timer = TimeSpan.Zero;

            // When
            sut
                .Timer(TimeSpan.FromSeconds(3))
                .Subscribe(x =>
                {
                    timer = x;
                });

            testScheduler.AdvanceByMs(3001);

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(0));
        }

        [Fact]
        public void Should_Resume_Where_Paused()
        {
            // Given
            var testScheduler = new TestScheduler();
            DecrementTimer sut = new DecrementTimerFixture().WithScheduler(testScheduler);
            var timer = TimeSpan.Zero;

            // When
            sut
                .Timer(TimeSpan.FromSeconds(30))
                .Subscribe(x =>
                {
                    timer = x;
                });

            testScheduler.AdvanceByMs(2001);
            timer.Should().Be(TimeSpan.FromSeconds(28));

            sut.Pause().Subscribe();
            testScheduler.AdvanceByMs(2000);
            timer.Should().Be(TimeSpan.FromSeconds(28));

            sut.Resume().Subscribe();
            testScheduler.AdvanceByMs(1001);

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(27));
        }

        [Fact]
        public void Should_Start_When_Started()
        {
            // Given
            var testScheduler = new TestScheduler();
            DecrementTimer sut = new DecrementTimerFixture().WithScheduler(testScheduler);
            var timer = TimeSpan.Zero;

            // When
            sut
                .Timer(TimeSpan.FromSeconds(3), false)
                .Subscribe(x =>
                {
                    timer = x;
                });

            testScheduler.AdvanceByMs(2000);
            timer.Should().Be(TimeSpan.Zero);
            sut.Start().Subscribe();
            testScheduler.AdvanceByMs(3001);

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(0));
        }

        public class TheIsRunningProperty
        {
            [Fact]
            public void Should_Return_True()
            {
                // Given
                var testScheduler = new TestScheduler();
                DecrementTimer sut = new DecrementTimerFixture().WithScheduler(testScheduler);

                // When
                sut.Timer(TimeSpan.FromSeconds(3)).Subscribe(x =>{ });
                testScheduler.AdvanceByMs(2000);

                // Then
                sut.IsRunning.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Resumed()
            {
                // Given
                var testScheduler = new TestScheduler();
                DecrementTimer sut = new DecrementTimerFixture().WithScheduler(testScheduler);

                // When
                sut.Timer(TimeSpan.FromSeconds(3), false).Subscribe(x =>{ });
                sut.IsRunning.Should().BeFalse();
                sut.Resume().Subscribe();

                // Then
                sut.IsRunning.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False()
            {
                // Given
                var testScheduler = new TestScheduler();
                DecrementTimer sut = new DecrementTimerFixture().WithScheduler(testScheduler);

                // When
                sut.Timer(TimeSpan.FromSeconds(3), false).Subscribe(x =>{ });
                testScheduler.AdvanceByMs(2000);

                // Then
                sut.IsRunning.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_False_When_Paused()
            {
                // Given
                var testScheduler = new TestScheduler();
                DecrementTimer sut = new DecrementTimerFixture().WithScheduler(testScheduler);

                // When
                sut.Timer(TimeSpan.FromSeconds(3)).Subscribe(x =>{ });
                sut.IsRunning.Should().BeTrue();
                sut.Pause().Subscribe();

                // Then
                sut.IsRunning.Should().BeFalse();
            }
        }
    }
}