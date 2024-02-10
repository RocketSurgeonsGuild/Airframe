using System;
using System.Reactive;
using System.Reactive.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Represents a <see cref="IStartupOperation"/> that logs using <see cref="ILogger{TCategoryName}"/>.
/// </summary>
[PublicAPI]
public abstract class StartupOperationBase : IStartupOperation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupOperationBase"/> class.
    /// </summary>
    /// <param name="factory">The logger factory.</param>
    protected StartupOperationBase(ILoggerFactory factory) => Logger = factory.CreateLogger(GetType());

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger Logger { get; }

    /// <inheritdoc/>
    IObservable<Unit> IStartupOperation.Start() =>
        Start().Finally(() => Logger.LogTrace("{StartupOperation}: Completed", GetType().Name));

    /// <inheritdoc/>
    IObservable<Unit> IOperation<Unit>.Execute() =>

        // Add logging.
        ((IStartupOperation)this).Start();

    /// <inheritdoc/>
    bool ICanExecute.CanExecute()
    {
        var canExecute = CanExecute();
        Logger.LogTrace("Can Execute: {CanExecute}", canExecute);
        return canExecute;
    }

    /// <summary>
    /// Template method for the startup operation.
    /// </summary>
    /// <returns>A completion notification.</returns>
    protected abstract IObservable<Unit> Start();

    /// <summary>
    /// Template method for whether or not the startup operation will execute.
    /// </summary>
    /// <returns>A completion notification.</returns>
    protected virtual bool CanExecute() => true;
}