namespace Rocket.Surgery.Airframe;

/// <summary>
/// Notification of a <see cref="IGpsLocation"/> update.
/// </summary>
public class LocationUpdatedEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocationUpdatedEvent"/> class.
    /// </summary>
    /// <param name="previous">The previous.</param>
    /// <param name="current">The current.</param>
    public LocationUpdatedEvent(IGpsLocation? previous, IGpsLocation current)
    {
        Previous = previous;
        Current = current;
    }

    /// <summary>
    /// Gets the previous location.
    /// </summary>
    public IGpsLocation? Previous { get; }

    /// <summary>
    /// Gets the current location.
    /// </summary>
    public IGpsLocation Current { get; }
}