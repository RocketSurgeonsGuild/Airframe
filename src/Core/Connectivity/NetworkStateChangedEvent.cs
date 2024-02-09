using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Airframe.Connectivity
{
    public class NetworkStateChangedEvent : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkStateChangedEvent"/> class.
        /// </summary>
        /// <param name="access"></param>
        /// <param name="connectionProfiles"></param>
        public NetworkStateChangedEvent(NetworkAccess access, params ConnectionProfile[] connectionProfiles)
        {
            NetworkAccess = access;
            ConnectionProfiles = connectionProfiles;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkStateChangedEvent"/> class.
        /// </summary>
        /// <param name="access">The current network access.</param>
        /// <param name="connectionProfiles"></param>
        public NetworkStateChangedEvent(NetworkAccess access, IEnumerable<ConnectionProfile> connectionProfiles)
            : this(access, connectionProfiles.ToArray()) { }

        public NetworkAccess NetworkAccess { get; }

        public IReadOnlyCollection<ConnectionProfile> ConnectionProfiles { get; }

        public bool HasSignal() => HasValidProfile() && HasNetworkAccess();

        /// <summary>
        /// Determines if the signal is degraded.
        /// </summary>
        /// <returns></returns>
        public bool Degraded() => NetworkAccess > NetworkAccess.None || ContainsProfile(ConnectionProfile.Unknown);

        /// <summary>
        /// Determines if this instance has a valid connection profile.
        /// </summary>
        /// <returns>A value indicating a valid profile exists.</returns>
        public bool HasValidProfile() => ContainsProfile(
            ConnectionProfile.Bluetooth,
            ConnectionProfile.Cellular,
            ConnectionProfile.Ethernet,
            ConnectionProfile.WiFi);

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(NetworkAccess)}: {NetworkAccess}, "
          + $"{nameof(ConnectionProfiles)}: [{string.Join(", ", ConnectionProfiles)}]";

        private bool HasNetworkAccess() => NetworkAccess is NetworkAccess.Internet or NetworkAccess.Local or NetworkAccess.ConstrainedInternet;

        private bool ContainsProfile(params ConnectionProfile[] profiles) =>
            ConnectionProfiles.Join(
                profiles,
                instanceProfile => instanceProfile,
                argumentProfile => argumentProfile,
                (instanceProfile, argumentProfile) => instanceProfile == argumentProfile)
           .Any(truth => truth);
    }
}