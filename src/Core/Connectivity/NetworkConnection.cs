namespace Rocket.Surgery.Airframe;

/// <summary>
/// The connection providing network access.
/// </summary>
public enum NetworkConnection
{
    /// <summary>
    /// An unknown connection.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// A bluetooth connection.
    /// </summary>
    Bluetooth = 1,

    /// <summary>
    /// A cellular connection.
    /// </summary>
    Cellular = 2,

    /// <summary>
    /// A wired connection.
    /// </summary>
    Ethernet = 3,

    /// <summary>
    /// A wireless connection.
    /// </summary>
    WiFi = 4
}