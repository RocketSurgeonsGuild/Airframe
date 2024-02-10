using System;

namespace Rocket.Surgery.Airframe.ViewModels;

/// <summary>
/// Interface representing an element that subscribes to network state changes.
/// </summary>
public interface IHaveNetworkState
{
    /// <summary>
    /// Gets the network state changes.
    /// </summary>
    IObservable<NetworkStateChangedEvent> NetworkState { get; }
}