using System;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Airframe.Exceptions;

/// <summary>
/// Represents a base abstraction for an <see cref="IUnhandledExceptionHandler"/>.
/// </summary>
public abstract class UnhandledExceptionHandlerBase : IUnhandledExceptionHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledExceptionHandlerBase"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    protected UnhandledExceptionHandlerBase(ILoggerFactory loggerFactory) => Logger = loggerFactory.CreateLogger(GetType());

    /// <inheritdoc/>
    public void HandleException(Exception exception, Guid correlationId)
    {
        if (!CanHandle(exception))
        {
            Logger.LogTrace("Cannot Handle: {Correlation}", correlationId);
            return;
        }

        Logger.LogTrace("Can Handle: {Correlation}", correlationId);
        Handle(exception, correlationId);
        Logger.LogTrace("Handled: {Correlation}", correlationId);
    }

    /// <inheritdoc/>
    bool IUnhandledExceptionHandler.CanHandle(Exception exception) => CanHandle(exception);

    /// <inheritdoc/>
    bool IUnhandledExceptionHandler.ShouldGobble(Exception exception) => ShouldGobble(exception);

    /// <summary>
    /// Handle the provided exception with a correlation identifier.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="correlationId">The correlation <see cref="Guid"/> scoped to this exception.</param>
    protected abstract void Handle(Exception exception, Guid correlationId);

    /// <summary>
    /// Determines if the <see cref="IExceptionHandler"/> can handle a given exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>returns <c>true</c> if the handler can handle it, otherwise <c>false</c>.</returns>
    protected abstract bool CanHandle(Exception exception);

    /// <summary>
    /// Determines if the <see cref="IExceptionHandler"/> should swallow the provided exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>returns <c>true</c> if the handler should swallow it, otherwise <c>false</c>.</returns>
    protected virtual bool ShouldGobble(Exception exception) => false;

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger Logger { get; }
}