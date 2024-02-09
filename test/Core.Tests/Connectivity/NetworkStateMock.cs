using Rocket.Surgery.Airframe.Connectivity;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Core.Tests.Connectivity;

internal class NetworkStateMock : INetworkState
{
    public NetworkAccess NetworkAccess { get; } = NetworkAccess.Internet;

    public IReadOnlyList<ConnectionProfile> Profiles { get; } = new[] { ConnectionProfile.WiFi };

    public IObservable<NetworkStateChangedEvent> ConnectivityChanged => _connectivity.AsObservable();

    public void Notify(NetworkStateChangedEvent networkStateChangedEvent) => _connectivity.OnNext(networkStateChangedEvent);

    public IDisposable Subscribe(IObserver<NetworkStateChangedEvent> observer) => _connectivity.Subscribe(observer);

    private readonly Subject<NetworkStateChangedEvent> _connectivity = new Subject<NetworkStateChangedEvent>();
}