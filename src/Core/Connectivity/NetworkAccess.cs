namespace Rocket.Surgery.Airframe;

/// <summary>
/// State of network availability.
/// </summary>
public enum NetworkAccess
{
    /// <summary>
    /// Cannot determine the state of the network.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// No network.
    /// </summary>
    None = 1,

    /// <summary>
    /// Local access only.
    /// </summary>
    Local = 2,

    /// <summary>
    /// Limited access.
    /// </summary>
    ConstrainedInternet = 3,

    /// <summary>
    /// Local and internet access.
    /// </summary>
    Internet = 4
}