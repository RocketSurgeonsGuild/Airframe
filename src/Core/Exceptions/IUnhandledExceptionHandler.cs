using System;

namespace Rocket.Surgery.Airframe.Exceptions;

/// <summary>
/// Interface representing an exception handler for an unhandled exception.
/// </summary>
public interface IUnhandledExceptionHandler
{
    /// <summary>
    /// Handle the provided exception.
    /// </summary>
    /// <param name="exception">The exception..</param>
    void HandleException(Exception exception) => HandleException(exception, Guid.NewGuid());

    /// <summary>
    /// Handle the provided exception with a correlation identifier.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="correlationId">the correlation <see cref="Guid"/> scoped to this exception.</param>
    void HandleException(Exception exception, Guid correlationId);

    /// <summary>
    /// Checks to see if this <see cref="IExceptionHandler"/> can handle a given exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>returns <c>true</c> if the handler can handle it, otherwise <c>false</c>.</returns>
    bool CanHandle(Exception exception);

    /// <summary>
    /// Checks to see if this <see cref="IExceptionHandler"/> should swallow a given exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>returns <c>true</c> if the handler should swallow it, otherwise <c>false</c>.</returns>
    bool ShouldGobble(Exception exception);
}