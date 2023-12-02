using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using Xunit;

namespace Rocket.Surgery.Airframe.Core.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1312:Variable names should begin with lower-case letter", Justification = "Discarded variable.")]
    public class ApplicationStartupTest
    {
        [Fact]
        public void GivenOperation_WhenStartupComplete_ThenCompletionRecieved()
        {
            // Given
            Unit? result = null;
            var testScheduler = new TestScheduler();
            var sut =
                new ApplicationStartupFixture()
                   .WithStartupOperations(new ScheduledTestOperation(testScheduler, TimeSpan.FromSeconds(3)))
                   .AsInterface();

            // When
            using var _ =
                sut.Startup()
                   .Subscribe(unit =>
                        {
                            // Then
                            result = unit;
                        });
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
            var sut = new ApplicationStartupFixture().WithStartupOperations(testOperation).AsInterface();

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
            var sut = new ApplicationStartupFixture().WithStartupOperations(testOperation).AsInterface();

            // When
            using var _ = sut.Startup().Subscribe();

            // Then
            testOperation.Executed.Should().BeTrue();
        }

        [Fact]
        public void GivenScheduler_WhenStartup_ThenExecuted()
        {
            // Given
            var testScheduler = new TestScheduler();
            var testOperation = new TestOperation();
            var sut = new ApplicationStartupFixture().WithStartupOperations(testOperation).AsInterface();
            using var _ = sut.Startup(1, testScheduler).Subscribe();

            // When
            testScheduler.Start();

            // Then
            testOperation.Executed.Should().BeTrue();
        }

        [Fact]
        public void GivenScheduler_WhenStartup_ThenNotExecuted()
        {
            // Given
            var testScheduler = new TestScheduler();
            var testOperation = new TestOperation();
            var sut = new ApplicationStartupFixture().WithStartupOperations(testOperation).AsInterface();

            // When
            using var _ = sut.Startup(1, testScheduler).Subscribe();

            // Then
            testOperation.Executed.Should().BeFalse();
        }
    }
}