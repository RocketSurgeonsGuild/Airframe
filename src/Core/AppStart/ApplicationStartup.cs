using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents the application startup sequence.
    /// </summary>
    public sealed class ApplicationStartup : IApplicationStartup
    {
        private readonly IEnumerable<IStartupOperation> _startupTasks;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationStartup"/> class.
        /// </summary>
        /// <param name="startupTasks">The startup tasks.</param>
        public ApplicationStartup(IEnumerable<IStartupOperation> startupTasks) =>
            _startupTasks = startupTasks;

        /// <inheritdoc/>
        public IObservable<Unit> Startup() => _startupTasks
            .Where(x => x.CanExecute())
            .Select(x => x.Start())
            .Concat();
    }
}