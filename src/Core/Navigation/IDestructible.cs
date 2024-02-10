namespace Rocket.Surgery.Airframe.Navigation;

/// <summary>
/// Interface that represents an element that needs to be destroyed.
/// </summary>
public interface IDestructible
{
    /// <summary>
    /// Tear down resources.
    /// </summary>
    void Destroy();
}