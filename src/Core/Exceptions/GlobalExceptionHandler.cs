using System.Collections.Generic;
using System.Reactive.Concurrency;
using JetBrains.Annotations;

namespace Rocket.Surgery.Airframe.Exceptions;

/// <summary>
/// Represents the default <see cref="IExceptionHandler"/> behavior for a global exception handler.
/// </summary>
[PublicAPI]
public sealed class GlobalExceptionHandler : GlobalExceptionHandlerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
    /// </summary>
    /// <param name="userInterfaceScheduler">The user interface thread scheduler.</param>
    /// <param name="exceptionHandlers">The enumerable of exception handlers.</param>
    public GlobalExceptionHandler(IScheduler userInterfaceScheduler, IEnumerable<IUnhandledExceptionHandler> exceptionHandlers)
        : base(userInterfaceScheduler, exceptionHandlers)
    {
    }
}