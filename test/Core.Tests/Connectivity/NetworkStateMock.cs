using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Core.Tests.Connectivity;

internal class NetworkStateMock : INetworkState
{
    public NetworkAccess NetworkAccess { get; } = NetworkAccess.Internet;

    public IReadOnlyList<NetworkConnection> Profiles { get; } = new[] { NetworkConnection.WiFi };

    public void Notify(NetworkStateChangedEvent networkStateChangedEvent) => _networkState.OnNext(networkStateChangedEvent);

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<NetworkStateChangedEvent> observer) => _networkState.Subscribe(observer);

    private readonly Subject<NetworkStateChangedEvent> _networkState = new Subject<NetworkStateChangedEvent>();
}