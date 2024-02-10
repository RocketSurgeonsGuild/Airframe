using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Class representing the <see cref="EventArgs"/> for network state changes.
/// </summary>
public class NetworkStateChangedEvent : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkStateChangedEvent"/> class.
    /// </summary>
    /// <param name="access">The current network access.</param>
    /// <param name="connections">The connection profile.</param>
    public NetworkStateChangedEvent(NetworkAccess access, params NetworkConnection[] connections)
    {
        Access = access;
        Connections = connections;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkStateChangedEvent"/> class.
    /// </summary>
    /// <param name="access">The current network access.</param>
    /// <param name="connectionProfiles">The connection profile.</param>
    public NetworkStateChangedEvent(NetworkAccess access, IEnumerable<NetworkConnection> connectionProfiles)
        : this(access, connectionProfiles.ToArray())
    {
    }

    /// <summary>
    /// Gets the network access.
    /// </summary>
    public NetworkAccess Access { get; }

    /// <summary>
    /// Gets the connection profiles.
    /// </summary>
    public IReadOnlyCollection<NetworkConnection> Connections { get; }

    /// <summary>
    /// Gets a value indicating whether there is a signal.
    /// </summary>
    /// <returns>A signal.</returns>
    public bool HasSignal() => HasValidConnection() && HasNetworkAccess();

    /// <summary>
    /// Determines if the signal is degraded.
    /// </summary>
    /// <returns>Degraded signal.</returns>
    public bool Degraded() => Access > NetworkAccess.None || ContainsConnection(NetworkConnection.Unknown);

    /// <summary>
    /// Determines if this instance has a valid connection profile.
    /// </summary>
    /// <returns>A value indicating a valid profile exists.</returns>
    public bool HasValidConnection() => ContainsConnection(
        NetworkConnection.Bluetooth,
        NetworkConnection.Cellular,
        NetworkConnection.Ethernet,
        NetworkConnection.WiFi);

    /// <inheritdoc/>
    public override string ToString() => $"{nameof(Access)}: {Access}, "
      + $"{nameof(Connections)}: [{string.Join(", ", Connections)}]";

    private bool HasNetworkAccess() => Access is NetworkAccess.Internet or NetworkAccess.Local or NetworkAccess.ConstrainedInternet;

    private bool ContainsConnection(params NetworkConnection[] profiles) =>
        Connections.Join(
                profiles,
                instanceProfile => instanceProfile,
                argumentProfile => argumentProfile,
                (instanceProfile, argumentProfile) => instanceProfile == argumentProfile)
           .Any(truth => truth);
}