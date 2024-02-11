using Airframe.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Airframe.Exceptions;
using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Core.Tests.Exceptions;

internal class FakeExceptionHandler : UnhandledExceptionHandlerBase, IHaveFakeResults<Exception>
{
    public FakeExceptionHandler(Func<bool> canHandle, Func<bool> shouldSwallow)
        : base(NullLoggerFactory.Instance)
    {
        _canHandle = canHandle;
        _shouldSwallow = shouldSwallow;
        Results = new List<Result<Exception>>();
    }

    public List<Result<Exception>> Results { get; }

    /// <inheritdoc/>
    protected override bool CanHandle(Exception exception) => _canHandle.Invoke();

    /// <inheritdoc/>
    protected override bool ShouldGobble(Exception exception) => _shouldSwallow.Invoke();

    /// <inheritdoc/>
    protected override void Handle(Exception exception, Guid correlationId) => Results.Add(new Result<Exception>(exception));

    private readonly Func<bool> _canHandle;
    private readonly Func<bool> _shouldSwallow;
}