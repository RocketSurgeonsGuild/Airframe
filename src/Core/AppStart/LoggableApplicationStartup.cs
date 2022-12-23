using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents the application startup sequence.
    /// </summary>
    public abstract class LoggableApplicationStartup : IApplicationStartup
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IStartupOperation> _startupOperations;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggableApplicationStartup"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="startupOperations">The startup tasks.</param>
        protected LoggableApplicationStartup(ILoggerFactory loggerFactory, IEnumerable<IStartupOperation> startupOperations)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _startupOperations = startupOperations;
        }

        /// <inheritdoc/>
        IObservable<Unit> IApplicationStartup.Startup(int concurrentOperations) => Startup(concurrentOperations)
           .Finally(() => _logger.LogTrace("{Startup} completed", GetType().Name));

        /// <inheritdoc/>
        IObservable<Unit> IApplicationStartup.Startup(int concurrentOperations, IScheduler scheduler) => null;

        /// <summary>
        /// Executes the provided <see cref="IStartupOperation"/>.
        /// </summary>
        /// <param name="concurrentOperations">The maximum concurrent operations.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>A completion notification of the startup operation execution.</returns>
        protected virtual IObservable<Unit> Startup(int concurrentOperations, IScheduler scheduler) => _startupOperations
           .Where(operation => operation.CanExecute())
           .Select(operation => operation.Start())
           .Merge(concurrentOperations, scheduler)
           .PublishLast()
           .RefCount();

        /// <summary>
        /// Executes the provided <see cref="IStartupOperation"/>.
        /// </summary>
        /// <param name="concurrentOperations">The maximum concurrent operations.</param>
        /// <returns>A completion notification of the startup operation execution.</returns>
        protected virtual IObservable<Unit> Startup(int concurrentOperations = 1) => Startup(concurrentOperations, CurrentThreadScheduler.Instance);
    }
}