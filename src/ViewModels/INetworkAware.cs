using System;

namespace Rocket.Surgery.Airframe.ViewModels;

/// <summary>
/// Interface representing an element that subscribes to network connectivity changes.
/// </summary>
public interface INetworkAware
{
    /// <summary>
    /// Gets the connectivity.
    /// </summary>
    IObservable<NetworkStateChangedEvent> Connectivity { get; }
}