using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Represents the application startup sequence.
/// </summary>
public abstract class ApplicationStartupBase : IApplicationStartup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationStartupBase"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="startupOperations">The startup tasks.</param>
    protected ApplicationStartupBase(ILoggerFactory loggerFactory, IEnumerable<IStartupOperation> startupOperations)
    {
        _logger = loggerFactory.CreateLogger(GetType());
        _startupOperations = startupOperations;
    }

    /// <inheritdoc/>
    IObservable<Unit> IApplicationStartup.Startup(int concurrentOperations) => Startup(_startupOperations, concurrentOperations, CurrentThreadScheduler.Instance)
       .Finally(() => _logger.LogTrace("Start Complete: {Startup}", GetType().Name));

    /// <inheritdoc/>
    IObservable<Unit> IApplicationStartup.Startup(int concurrentOperations, IScheduler scheduler) => Startup(_startupOperations, concurrentOperations, scheduler)
       .Finally(() => _logger.LogTrace("Start Complete: {Startup}", GetType().Name));

    /// <summary>
    /// Executes the provided <see cref="IStartupOperation"/>.
    /// </summary>
    /// <param name="operations">The startup operations.</param>
    /// <param name="concurrentOperations">The maximum concurrent operations.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <returns>A completion notification of the startup operation execution.</returns>
    protected virtual IObservable<Unit> Startup(IEnumerable<IStartupOperation> operations, int concurrentOperations, IScheduler scheduler) => operations
       .Where(operation => operation.CanExecute())
       .Select(operation => operation.Start())
       .Merge(concurrentOperations, scheduler)
       .PublishLast()
       .RefCount();

    /// <summary>
    /// Executes the provided <see cref="IStartupOperation"/>.
    /// </summary>
    /// <param name="operations">The startup operations.</param>
    /// <param name="concurrentOperations">The maximum concurrent operations.</param>
    /// <returns>A completion notification of the startup operation execution.</returns>
    protected virtual IObservable<Unit> Startup(IEnumerable<IStartupOperation> operations, int concurrentOperations = 1) => Startup(operations, concurrentOperations, CurrentThreadScheduler.Instance);

    private readonly ILogger _logger;
    private readonly IEnumerable<IStartupOperation> _startupOperations;
}