namespace Rocket.Surgery.Airframe.Navigation;

/// <summary>
/// Interface representing an initializeable thing™.
/// </summary>
public interface IInitialize
{
    /// <summary>
    /// Executed when an element is created.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    void OnInitialize(IArguments arguments);
}