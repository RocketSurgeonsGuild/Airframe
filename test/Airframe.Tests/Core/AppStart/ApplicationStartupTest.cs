using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Rocket.Surgery.Airframe;
using System;
using System.Reactive.Linq;
using Xunit;

namespace Airframe.Tests.Core.AppStart
{
    public class ApplicationStartupTest : TestBase
    {
        [Fact]
        public void Should_Notify_When_Operations_Complete()
        {
            // Given
            var result = false;
            var testScheduler = new TestScheduler();
            ApplicationStartup sut = new ApplicationStartupFixture().WithStartupOperations(new TestOperation(testScheduler, TimeSpan.FromSeconds(3)));

            // When
            sut.Startup()
               .Select(_ => true)
               .Subscribe(
                    x =>
                    {
                        // Then
                        result = x;
                    }
                );
            testScheduler.AdvanceByMs(1000);

            result.Should().BeFalse();

            testScheduler.AdvanceByMs(2001);
            result.Should().BeTrue();
        }


        [Fact]
        public void Should_Skip_If_Cannot_Execute()
        {
            // Given
            var testScheduler = new TestScheduler();
            ApplicationStartup sut = new ApplicationStartupFixture().WithStartupOperations(new TestOperation(testScheduler, TimeSpan.FromSeconds(3), false));

            // When
            sut.Startup()
               .Select(_ => true)
               .Subscribe(
                    x =>
                    {
                        // Then
                        x.Should().BeTrue();
                    }
                );
        }
    }
}