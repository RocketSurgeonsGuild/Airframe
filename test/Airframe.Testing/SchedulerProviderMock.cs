using Microsoft.Reactive.Testing;
using Rocket.Surgery.Airframe;
using System.Reactive.Concurrency;

namespace Airframe.Testing;

public sealed class SchedulerProviderMock : ISchedulerProvider
{
    internal SchedulerProviderMock(IScheduler userInterfaceTestScheduler, IScheduler backgroundTestScheduler)
    {
        UserInterfaceTestScheduler = userInterfaceTestScheduler as TestScheduler ?? throw new ArgumentOutOfRangeException(nameof(userInterfaceTestScheduler));
        BackgroundTestScheduler = backgroundTestScheduler as TestScheduler ?? throw new ArgumentOutOfRangeException(nameof(userInterfaceTestScheduler));
    }

    /// <inheritdoc/>
    IScheduler ISchedulerProvider.UserInterfaceThread => UserInterfaceTestScheduler;

    /// <inheritdoc/>
    IScheduler ISchedulerProvider.BackgroundThread => BackgroundTestScheduler;

    /// <summary>
    /// Gets the test user interface scheduler.
    /// </summary>
    public TestScheduler UserInterfaceTestScheduler { get; }

    /// <summary>
    /// Gets the test background scheduler.
    /// </summary>
    public TestScheduler BackgroundTestScheduler { get; }
}