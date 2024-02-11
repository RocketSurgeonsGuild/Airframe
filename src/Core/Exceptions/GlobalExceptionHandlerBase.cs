using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Rocket.Surgery.Airframe.Exceptions;

/// <summary>
/// Represents the base abstraction for <see cref="IExceptionHandler"/>.
/// </summary>
public abstract class GlobalExceptionHandlerBase : IExceptionHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandlerBase"/> class.
    /// </summary>
    /// <param name="userInterfaceScheduler">The user interface thread scheduler.</param>
    /// <param name="exceptionHandlers">The enumerable of exception handlers.</param>
    protected GlobalExceptionHandlerBase(IScheduler userInterfaceScheduler, IEnumerable<IUnhandledExceptionHandler> exceptionHandlers)
    {
        _scheduler = userInterfaceScheduler;
        _exceptionHandlers = exceptionHandlers;
    }

    /// <inheritdoc/>
    void IObserver<Exception>.OnCompleted() => _scheduler.Schedule(() => OnCompleted());

    /// <inheritdoc/>
    void IObserver<Exception>.OnError(Exception error) => _scheduler.Schedule(
        () => OnError(error),
        (_, state) =>
        {
            state.Invoke();
            return Disposable.Empty;
        });

    /// <inheritdoc/>
    void IObserver<Exception>.OnNext(Exception value) => OnNext(value);

    /// <summary>Provides the observer with new data.</summary>
    /// <param name="value">The current notification information.</param>
    protected virtual void OnNext(Exception value)
    {
        foreach (var exceptionHandler in _exceptionHandlers)
        {
            try
            {
                exceptionHandler.HandleException(value);

                if (exceptionHandler.ShouldGobble(value))
                {
                    break;
                }
            }
            catch (Exception exception)
            {
                // the handler failed
                OnError(
                    new AggregateException(
                        "The list exception handlers has failed to handle the provided exception. See inner exceptions for details",
                        exception,
                        value));
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandlerBase"/> class.
    /// Notifies the observer that the provider has experienced an error condition.
    /// </summary>
    /// <param name="error">An object that provides additional information about the error.</param>
    protected virtual void OnError(Exception error) => _scheduler.Schedule(() => throw error);

    /// <summary>
    /// Notifies the observer that the provider has finished sending push-based notifications.
    /// </summary>
    protected virtual void OnCompleted() => _scheduler.Schedule(() => throw new NotImplementedException());

    private readonly IScheduler _scheduler;
    private readonly IEnumerable<IUnhandledExceptionHandler> _exceptionHandlers;
}