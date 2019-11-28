using System;
using System.Collections.Generic;
using System.Text;
using Rocket.Surgery.Xamarin.Essentials;
using Rocket.Surgery.Xamarin.Essentials.Abstractions;
using Xamarin.Essentials;

namespace Rocket.Surgery.Airframe.ViewModels
{
    /// <summary>
    /// Interface representing an element that subscribes to network connectivity changes.
    /// </summary>
    public interface INetworkAware
    {
        /// <summary>
        /// Gets the connectivity.
        /// </summary>
        IObservable<ConnectivityChangedEventArgs> Connectivity { get; }
    }
}
