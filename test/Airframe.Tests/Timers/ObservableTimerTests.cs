using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using ReactiveUI.Testing;
using Rocket.Surgery.Airframe.Timers;
using Xunit;

namespace Airframe.Tests.Timers
{
    public class ObservableTimerTests : TestBase
    {
        const int InitialMilliseconds = 1000;
        const int OneThousandMilliseconds = 1000;

        public ObservableTimerTests()
        {
            RxApp.DefaultExceptionHandler = BaseExceptionHandlerStub.Instance;
        }

        [Fact]
        public void Should_Not_Start_When_Set()
        {
            // Given
            ObservableTimer sut = new ObservableTimerFixture();
            var timer = TimeSpan.Zero;
            sut.Subscribe(x =>
            {
                timer = x;
            });

            // When
            sut.Set(TimeSpan.FromMinutes(25));

            // Then
            timer.Should().Be(TimeSpan.Zero);
        }
        
        [Fact]
        public void Should_Not_Be_Running_When_Constructed()
        {
            // Given
            ObservableTimer sut = new ObservableTimerFixture();
            var timer = TimeSpan.Zero;
            sut.Subscribe(x =>
            {
                timer = x;
            });

            // When
            sut.Set(TimeSpan.FromMinutes(25));

            // Then
            sut.IsRunning.Should().BeFalse();
        }

        [Fact]
        public void Should_Be_Running_When_Started()
        {
            // Given
            var testScheduler = new TestScheduler();
            var schedulerProvider = new SchedulerProviderMock(background: testScheduler);
            ObservableTimer sut = new ObservableTimerFixture().WithProvider(schedulerProvider);
            var timer = TimeSpan.Zero;
            sut.Subscribe(x =>
            {
                timer = x;
            });
            sut.Set(TimeSpan.FromMinutes(25));

            // When
            sut.Start();
            testScheduler.AdvanceByMs(20000);

            // Then
            sut.IsRunning.Should().BeTrue();
        }

        [Fact]
        public void Should_Advance_When_Started()
        {
            // Given
            var testScheduler = new TestScheduler();
            var schedulerProvider = new SchedulerProviderMock(background: testScheduler);
            ObservableTimer sut = new ObservableTimerFixture().WithProvider(schedulerProvider);
            var timer = TimeSpan.Zero;
            sut.Set(TimeSpan.FromMinutes(1));

            sut.Subscribe(x =>
            {
                timer = x;
            });

            // When
            sut.Start();
            testScheduler.AdvanceByMs(InitialMilliseconds);

            // Then
            timer.Should().Be(TimeSpan.FromSeconds(59));
        }

        
        [Fact]
        public void Should_Resume_Where_Stopped()
        {
            // Given
            var testScheduler = new TestScheduler();
            var schedulerProvider = new SchedulerProviderMock(background: testScheduler);
            ObservableTimer sut = new ObservableTimerFixture().WithProvider(schedulerProvider);
            var timer = TimeSpan.Zero;
            sut.Set(TimeSpan.FromMinutes(1));

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
            timer.Should().Be(TimeSpan.FromSeconds(59));
        }
        
        
        [Fact]
        public void Should_Resume_After_Stopped()
        {
            // Given
            var testScheduler = new TestScheduler();
            var schedulerProvider = new SchedulerProviderMock(background: testScheduler);
            ObservableTimer sut = new ObservableTimerFixture().WithProvider(schedulerProvider);
            var timer = TimeSpan.Zero;
            sut.Set(TimeSpan.FromMinutes(1));

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
            timer.Should().Be(TimeSpan.FromSeconds(58));
        }
    }
}