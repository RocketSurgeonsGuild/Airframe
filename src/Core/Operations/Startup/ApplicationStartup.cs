using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Represents the application startup sequence.
/// </summary>
public sealed class ApplicationStartup : LoggableApplicationStartup, IApplicationStartup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationStartup"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="startupOperations">The startup operations.</param>
    public ApplicationStartup(ILoggerFactory loggerFactory, IEnumerable<IStartupOperation> startupOperations)
        : base(loggerFactory, startupOperations)
    {
    }
}