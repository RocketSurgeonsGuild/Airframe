﻿using System.Reactive.Concurrency;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Interface representing a provider for <see cref="IScheduler"/> instances.
/// </summary>
public interface ISchedulerProvider
{
    /// <summary>
    /// Gets the user interface thread scheduler.
    /// </summary>
    IScheduler UserInterfaceThread { get; }

    /// <summary>
    /// Gets the background thread scheduler.
    /// </summary>
    IScheduler BackgroundThread { get; }
}