using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using System;
using System.Reactive;
using Xunit;

namespace Rocket.Surgery.Airframe.Core.Tests
{
    public class ApplicationStartupTest
    {
        [Fact]
        public void GivenOperation_WhenStartupComplete_ThenCompletionRecieved()
        {
            // Given
            Unit? result = null;
            var testScheduler = new TestScheduler();
            ApplicationStartup sut = new ApplicationStartupFixture().WithStartupOperations(new ScheduledTestOperation(testScheduler, TimeSpan.FromSeconds(3)));

            // When
            sut.Startup()
               .Subscribe(
                    x =>
                    {
                        // Then
                        result = x;
                    }
                );
            testScheduler.AdvanceByMs(1000);

            // Then
            result.Should().BeNull();
            testScheduler.AdvanceByMs(2001);
            result.Should().NotBeNull();
        }

        [Fact]
        public void GivenOperationCannotExecute_WhenStartup_ThenNotExecuted()
        {
            // Given
            var testOperation = new TestOperation(false);
            ApplicationStartup sut = new ApplicationStartupFixture().WithStartupOperations(testOperation);

            // When
            using var _ = sut.Startup().Subscribe();

            // Then
            testOperation.Executed.Should().BeFalse();
        }

        [Fact]
        public void GivenOperationCanExecute_WhenStartup_ThenExecuted()
        {
            // Given
            var testOperation = new TestOperation();
            ApplicationStartup sut = new ApplicationStartupFixture().WithStartupOperations(testOperation);

            // When
            using var _ = sut.Startup().Subscribe();

            // Then
            testOperation.Executed.Should().BeTrue();
        }
    }
}