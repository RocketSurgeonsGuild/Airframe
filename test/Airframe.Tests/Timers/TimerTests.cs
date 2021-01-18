using System;
using Core;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using ReactiveUI.Testing;
using Rocket.Surgery.Airframe.Timers;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using Xunit;

namespace Airframe.Tests.Timers
{
    public class TimerTests : TestBase
    {
        const int InitialMilliseconds = 1000;
        const int OneThousandMilliseconds = 1000;

        public TimerTests()
        {
            RxApp.DefaultExceptionHandler = BaseExceptionHandlerStub.Instance;
        }

        [Fact]
        public void Should_Not_Start_When_Set()
        {
            // Given
            Timer sut = new TimerFixture();
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
            Timer sut = new TimerFixture();
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
            Timer sut = new TimerFixture().WithProvider(schedulerProvider);
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
            Timer sut = new TimerFixture().WithProvider(schedulerProvider);
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
            Timer sut = new TimerFixture().WithProvider(schedulerProvider);
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
            Timer sut = new TimerFixture().WithProvider(schedulerProvider);
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

    internal class TimerFixture : ITestFixtureBuilder
    {
        private ISchedulerProvider _schedulerProvider = new SchedulerProviderMock();

        public static implicit operator Timer(TimerFixture fixture) => fixture.Build();

        public TimerFixture WithProvider(ISchedulerProvider schedulerProvider) =>
            this.With(ref _schedulerProvider, schedulerProvider);

        private Timer Build() => new Timer(_schedulerProvider);
    }
    
    internal sealed class BaseExceptionHandlerStub : IObserver<Exception>
    {
        public static readonly BaseExceptionHandlerStub Instance = new BaseExceptionHandlerStub();

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        /// <inheritdoc />
        public void OnNext(Exception exception) {}
    }

    public abstract class TestBase
    {
        protected TestBase()
        {
            Initialize();
        }

        private static void Initialize()
        {
            RxApp.DefaultExceptionHandler = BaseExceptionHandlerStub.Instance;
        }
    }
}