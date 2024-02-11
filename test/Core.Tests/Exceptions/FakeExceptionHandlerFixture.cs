using Rocket.Surgery.Extensions.Testing.Fixtures;
using System;

namespace Rocket.Surgery.Airframe.Core.Tests.Exceptions;

internal sealed class FakeExceptionHandlerFixture : ITestFixtureBuilder
{
    public static implicit operator FakeExceptionHandler(FakeExceptionHandlerFixture fixture) => fixture.Build();

    public FakeExceptionHandlerFixture WithCanHandle(Func<bool> canHandle) => this.With(ref _canHandleFunction, canHandle);

    public FakeExceptionHandlerFixture WithCanHandle(bool canHandle) => WithCanHandle(() => canHandle);

    public FakeExceptionHandlerFixture WithShouldGobble(Func<bool> shouldGobble) => this.With(ref _shouldGobbleFunction, shouldGobble);

    public FakeExceptionHandlerFixture WithShouldGobble(bool shouldGobble) => WithShouldGobble(() => shouldGobble);

    private FakeExceptionHandler Build() => new FakeExceptionHandler(_canHandleFunction, _shouldGobbleFunction);

    private Func<bool> _canHandleFunction = () => true;
    private Func<bool> _shouldGobbleFunction = () => false;
}