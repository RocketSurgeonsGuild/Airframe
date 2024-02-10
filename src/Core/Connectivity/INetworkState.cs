using System;
using System.Reactive.Linq;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Interface representing the Network Connectivity.
/// </summary>
public interface INetworkState : IObservable<NetworkStateChangedEvent>
{
    /// <summary>
    /// Gets an observable sequence notifying if the device has connectivity.
    /// </summary>
    /// <returns>A value indicating the is a signal.</returns>
    IObservable<bool> HasSignal() =>
        this
           .Select(changedEvent => changedEvent.HasSignal())
           .DistinctUntilChanged()
           .Publish()
           .RefCount();

    /// <summary>
    /// Gets an observable sequence notifying that signal has degraded.
    /// </summary>
    /// <returns>A value indicating the signal is degraded.</returns>
    IObservable<bool> SignalDegraded() =>
        this
           .DistinctUntilChanged(changedEvent => changedEvent.Access)
           .Select(changedEvent => changedEvent.Degraded())
           .DistinctUntilChanged()
           .Publish()
           .RefCount();

    /// <summary>
    /// Gets an observable sequence notifying if the device has network access.
    /// </summary>
    /// <returns>Network state events when there is a signal.</returns>
    IObservable<NetworkStateChangedEvent> WhereHasSignal() =>
        this
           .Where(changedEvent => changedEvent.HasSignal())
           .DistinctUntilChanged()
           .Publish()
           .RefCount();

    /// <summary>
    /// Gets an observable sequence notifying if the device has a signal.
    /// </summary>
    /// <returns>Network state events when there is no signal.</returns>
    IObservable<NetworkStateChangedEvent> WhereHasNoSignal() =>
        this
           .Where(changedEvent => !changedEvent.HasSignal())
           .DistinctUntilChanged()
           .Publish()
           .RefCount();
}