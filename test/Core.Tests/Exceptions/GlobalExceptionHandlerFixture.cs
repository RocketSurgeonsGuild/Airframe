using NSubstitute;
using Rocket.Surgery.Airframe.Exceptions;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;

namespace Rocket.Surgery.Airframe.Core.Tests.Exceptions;

internal sealed class GlobalExceptionHandlerFixture : ITestFixtureBuilder
{
    public static implicit operator GlobalExceptionHandler(GlobalExceptionHandlerFixture fixture) => fixture.Build();

    public GlobalExceptionHandlerFixture WithScheduler(IScheduler scheduler) => this.With(ref _scheduler, scheduler);

    public GlobalExceptionHandlerFixture WithHandlers(params IUnhandledExceptionHandler[] handlers) => this.With(ref _unhandledExceptionHandler, handlers);

    public IExceptionHandler AsInterface() => this.Build();

    private GlobalExceptionHandler Build() => new(_scheduler, _unhandledExceptionHandler);

    private IScheduler _scheduler = Substitute.For<IScheduler>();
    private IEnumerable<IUnhandledExceptionHandler> _unhandledExceptionHandler = Enumerable.Empty<IUnhandledExceptionHandler>();
}