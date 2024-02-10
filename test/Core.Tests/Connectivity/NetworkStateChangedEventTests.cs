using FluentAssertions;
using Xunit;

namespace Rocket.Surgery.Airframe.Core.Tests.Connectivity;

public class NetworkStateChangedEventTests
{
    [Theory]
    [InlineData(NetworkConnection.Bluetooth)]
    [InlineData(NetworkConnection.Ethernet)]
    [InlineData(NetworkConnection.Cellular)]
    [InlineData(NetworkConnection.WiFi)]
    public void GivenValidProfile_WhenHasValidProfile_ThenResultTrue(NetworkConnection profile)
    {
        // Given, When
        NetworkStateChangedEvent sut = new NetworkStateChangedEventFixture().WithProfiles(profile);

        // Then
        sut
           .HasValidConnection()
           .Should()
           .BeTrue();
    }

    [Theory]
    [InlineData(NetworkConnection.Unknown)]
    public void GivenValidProfile_WhenHasValidProfile_ThenResultFalse(NetworkConnection profile)
    {
        // Given, When
        NetworkStateChangedEvent sut = new NetworkStateChangedEventFixture().WithProfiles(profile);

        // Then
        sut
           .HasValidConnection()
           .Should()
           .BeFalse();
    }

    [Fact]
    public void GivenDefaultEventArg_WhenHasAccess_ThenResultTrue()
    {
        // Given, When
        NetworkStateChangedEvent sut = new NetworkStateChangedEventFixture().WithDefault();

        // Then
        sut.HasSignal()
           .Should()
           .BeTrue();
    }

    [Fact]
    public void GivenDefaultEventArg_WhenToString_ThenResultCorrect()
    {
        // Given, When
        NetworkStateChangedEvent sut = new NetworkStateChangedEventFixture().WithDefault();

        // Then
        sut.ToString()
           .Should()
           .Be("Access: Internet, Connections: [Bluetooth, Ethernet, WiFi]");
    }

    [Fact]
    public void GivenDefaultEventArg_WhenDegraded_ThenResultTrue()
    {
        // Given, When
        NetworkStateChangedEvent sut = new NetworkStateChangedEventFixture().WithProfiles(NetworkConnection.Unknown);

        // Then
        sut.Degraded()
           .Should()
           .BeTrue();
    }
}