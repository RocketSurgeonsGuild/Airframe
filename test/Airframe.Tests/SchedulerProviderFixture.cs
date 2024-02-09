using Microsoft.Reactive.Testing;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;

namespace Rocket.Surgery.Airframe.Tests
{
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order")]
    public class SchedulerProviderFixture : ITestFixtureBuilder
    {
        public static implicit operator SchedulerProviderMock(SchedulerProviderFixture fixture) => fixture.Build();

        public SchedulerProviderFixture WithTestScheduler(IScheduler scheduler) => this.With(ref _mainThreadScheduler, scheduler)
           .With(ref _backgroundThreadScheduler, scheduler);

        private SchedulerProviderMock Build() => new SchedulerProviderMock(_mainThreadScheduler, _backgroundThreadScheduler);

        private IScheduler _mainThreadScheduler = new TestScheduler();
        private IScheduler _backgroundThreadScheduler = new TestScheduler();

        public class SchedulerProviderMock(IScheduler userInterfaceTestScheduler, IScheduler backgroundTestScheduler) : ISchedulerProvider
        {
            /// <inheritdoc/>
            IScheduler ISchedulerProvider.UserInterfaceThread => UserInterfaceTestScheduler;

            /// <inheritdoc/>
            IScheduler ISchedulerProvider.BackgroundThread => BackgroundTestScheduler;

            /// <summary>
            /// Gets the test user interface scheduler.
            /// </summary>
            public TestScheduler UserInterfaceTestScheduler { get; } = userInterfaceTestScheduler as TestScheduler ?? throw new ArgumentOutOfRangeException(nameof(userInterfaceTestScheduler));

            /// <summary>
            /// Gets the test background scheduler.
            /// </summary>
            public TestScheduler BackgroundTestScheduler { get; } = backgroundTestScheduler as TestScheduler ?? throw new ArgumentOutOfRangeException(nameof(userInterfaceTestScheduler));
        }
    }
}