using System;
using System.Reactive.Concurrency;
using Core;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using ReactiveUI.Testing;
using Rocket.Surgery.Airframe.Exceptions;
using Rocket.Surgery.Airframe.Timers;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Airframe.Tests.Timers
{
    public class TimerTests : TestBase
    {
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
            sut.Set(TimeSpan.FromMinutes(25));

            sut.Subscribe(x =>
            {
                timer = x;
            });

            // When
            sut.Start();
            testScheduler.AdvanceByMs(20000);

            // Then
            timer.Should().NotBe(TimeSpan.Zero);
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
    
    internal sealed class BaseExceptionHandlerStub : ExceptionHandlerBase
    {
        public static readonly BaseExceptionHandlerStub Instance = new BaseExceptionHandlerStub();

        /// <inheritdoc />
        public override void OnNext(Exception exception) {}
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