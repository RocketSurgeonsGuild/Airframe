using FluentAssertions;
using Xunit;

namespace Rocket.Surgery.Airframe.Core.Tests.Listeners;

public class ListenerTests
{
    [Fact]
    public void GivenListening_When_ThenResultNotNull() =>

        // Given
        // When
        // Then
        true.Should().BeFalse();

    [Fact]
    public void GivenListening_WhenIgnore_ThenResultNull() =>

        // Given
        // When
        // Then
        true.Should().BeFalse();

    [Fact]
    public void GivenIgnore_When_ThenResultNull() =>

        // Given
        // When
        // Then
        true.Should().BeFalse();

    [Fact]
    public void GivenIgnore_WhenListen_ThenResultNotNull() =>

        // Given
        // When
        // Then
        true.Should().BeFalse();
}