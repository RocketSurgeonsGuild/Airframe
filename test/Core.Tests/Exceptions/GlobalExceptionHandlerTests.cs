using FluentAssertions;
using NSubstitute;
using Rocket.Surgery.Airframe.Exceptions;
using System;
using Xunit;

namespace Rocket.Surgery.Airframe.Core.Tests.Exceptions;

public class GlobalExceptionHandlerTests
{
    [Fact]
    public void GivenUnhandledExceptionHandler_WhenExceptionThrown_ThenHandled()
    {
        // Given
        var exception = new Exception();
        var handler = Substitute.For<IUnhandledExceptionHandler>();
        handler.CanHandle(Arg.Any<Exception>()).Returns(true);
        var sut = new GlobalExceptionHandlerFixture().WithHandlers(handler).AsInterface();

        // When
        sut.OnNext(exception);

        // Then
        handler.Received(1).HandleException(exception);
    }

    [Fact]
    public void GivenUnhandledExceptionHandler_WhenCannotHandle_ThenNotHandled()
    {
        // Given
        var exception = new Exception();
        FakeExceptionHandler handler = new FakeExceptionHandlerFixture().WithCanHandle(false);
        var sut = new GlobalExceptionHandlerFixture().WithHandlers(handler.As<IUnhandledExceptionHandler>()).AsInterface();

        // When
        sut.OnNext(exception);

        // Then
        handler.Results.Should().BeEmpty();
    }

    [Fact]
    public void GivenUnhandledExceptionHandler_WhenCanHandle_ThenHandled()
    {
        // Given
        var exception = new Exception();
        FakeExceptionHandler handler = new FakeExceptionHandlerFixture();
        var sut = new GlobalExceptionHandlerFixture().WithHandlers(handler).AsInterface();

        // When
        sut.OnNext(exception);

        // Then
        handler.Results.Should().ContainSingle(result => !string.IsNullOrWhiteSpace(result.Value.Message));
    }

    [Fact]
    public void GivenMultipleHandler_WhenSomeCanHandle_ThenCorrectHandlersHandle()
    {
        // Given
        var exception = new Exception();
        FakeExceptionHandler handler1 = new FakeExceptionHandlerFixture();
        FakeExceptionHandler handler2 = new FakeExceptionHandlerFixture().WithCanHandle(() => false);
        var sut = new GlobalExceptionHandlerFixture().WithHandlers(handler1, handler2).AsInterface();

        // When
        sut.OnNext(exception);

        // Then
        handler1.Results.Should().ContainSingle(result => !string.IsNullOrWhiteSpace(result.Value.Message));
        handler2.Results.Should().BeEmpty();
    }

    [Fact]
    public void GivenMultipleHandler_WhenShouldGobble_ThenCorrectHandlerCalled()
    {
        // Given
        var exception = new Exception();
        FakeExceptionHandler handler1 = new FakeExceptionHandlerFixture().WithCanHandle(true).WithShouldGobble(true);
        FakeExceptionHandler handler2 = new FakeExceptionHandlerFixture().WithCanHandle(true).WithShouldGobble(false);
        var sut = new GlobalExceptionHandlerFixture().WithHandlers(handler1, handler2).AsInterface();

        // When
        sut.OnNext(exception);

        // Then
        handler1.Results.Should().ContainSingle(result => !string.IsNullOrWhiteSpace(result.Value.Message));
        handler2.Results.Should().BeEmpty();
    }

    [Fact]
    public void GivenShouldGobble_WhenOnNext_ThenResultsEmpty()
    {
        // Given
        var exception = new Exception();
        FakeExceptionHandler handler = new FakeExceptionHandlerFixture().WithCanHandle(true).WithShouldGobble(true);
        var sut = new GlobalExceptionHandlerFixture().WithHandlers(handler).AsInterface();

        // When
        sut.OnNext(exception);

        // Then
        handler.Results.Should().NotBeEmpty();
    }
}