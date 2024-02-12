using FluentAssertions;
using System;
using System.Reactive;
using Xunit;

namespace Rocket.Surgery.Airframe.Core.Tests.Listeners;

public class ListenerTests
{
    [Fact]
    public void GivenListening_When_ThenResultNotNull()
    {
        // Given
        Unit? result = null;
        TestListener sut = new TestListenerFixture();
        using var _ = sut.As<IListener>().Listen().Subscribe(actual => result = actual);

        // When
        sut.Notify(Unit.Default);

        // Then
        result.Should().NotBeNull();
    }

    [Fact]
    public void GivenListening_WhenIgnore_ThenResultNull()
    {
        // Given
        Unit? result = null;
        TestListener sut = new TestListenerFixture();
        using var _ = sut.As<IListener>().Listen().Subscribe(actual => result = actual);

        // When
        using var __ = sut.As<IListener>().Ignore().Subscribe();
        sut.Notify(Unit.Default);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    public void GivenIgnore_WhenListen_ThenResultNotNull()
    {
        // Given
        Unit? result = null;
        TestListener sut = new TestListenerFixture();
        using var _ = sut.As<IListener>().Ignore().Subscribe();

        // When
        using var __ = sut.As<IListener>().Listen().Subscribe(actual => result = actual);
        sut.Notify(Unit.Default);

        // Then
        result.Should().NotBeNull();
    }
}