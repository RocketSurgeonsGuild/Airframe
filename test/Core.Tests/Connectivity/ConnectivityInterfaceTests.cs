using FluentAssertions;
using Rocket.Surgery.Airframe.Connectivity;
using System;
using Xunit;

namespace Rocket.Surgery.Airframe.Core.Tests.Connectivity;

public class ConnectivityInterfaceTests
{
    [Theory]
    [InlineData(NetworkAccess.Internet)]
    [InlineData(NetworkAccess.Local)]
    [InlineData(NetworkAccess.ConstrainedInternet)]
    public void GivenImplementation_WhenConnectivityChanged_ThenHasSignal(NetworkAccess access)
    {
        // Given
        var sut = new NetworkStateMock();
        bool? result = null;
        using var _ = sut.As<INetworkState>().HasSignal().Subscribe(actual => result = actual);

        // When
        sut.Notify(new NetworkStateChangedEvent(access, ConnectionProfile.WiFi));

        // Then
        result
           .Should()
           .NotBeNull()
           .And
           .Subject
           .Should()
           .BeTrue();
    }

    [Theory]
    [InlineData(NetworkAccess.None)]
    [InlineData(NetworkAccess.Unknown)]
    public void GivenImplementation_WhenConnectivityChanged_ThenHasNoSignal(NetworkAccess access)
    {
        // Given
        var sut = new NetworkStateMock();
        bool? result = null;
        using var _ = sut.As<INetworkState>().HasSignal().Subscribe(actual => result = actual);

        // When
        sut.Notify(new NetworkStateChangedEvent(access));

        // Then
        result
           .Should()
           .NotBeNull()
           .And
           .Subject
           .Should()
           .BeFalse();
    }

    [Theory]
    [InlineData(NetworkAccess.Internet)]
    [InlineData(NetworkAccess.Local)]
    [InlineData(NetworkAccess.ConstrainedInternet)]
    public void GivenImplementation_WhenConnectivityChanged_ThenSignalDegraded(NetworkAccess access)
    {
        // Given
        var sut = new NetworkStateMock();
        bool? result = null;
        using var _ = sut.As<INetworkState>().SignalDegraded().Subscribe(actual => result = actual);

        // When
        sut.Notify(new NetworkStateChangedEvent(access));

        // Then
        result
           .Should()
           .NotBeNull()
           .And
           .Subject
           .Should()
           .BeTrue();
    }

    [Theory]
    [InlineData(NetworkAccess.None)]
    [InlineData(NetworkAccess.Unknown)]
    public void GivenImplementation_WhenConnectivityChanged_ThenSignalNotDegraded(NetworkAccess access)
    {
        // Given
        var sut = new NetworkStateMock();
        bool? result = null;
        using var _ = sut.As<INetworkState>().SignalDegraded().Subscribe(actual => result = actual);

        // When
        sut.Notify(new NetworkStateChangedEvent(access));

        // Then
        result
           .Should()
           .NotBeNull()
           .And
           .Subject
           .Should()
           .BeFalse();
    }
}