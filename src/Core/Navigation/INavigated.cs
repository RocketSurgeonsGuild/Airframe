namespace Rocket.Surgery.Airframe.Navigation;

/// <summary>
/// Interface representing a thing™ that can be navigated to and from.
/// </summary>
public interface INavigated
{
    /// <summary>
    /// Executed when an element is navigated to.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    void OnNavigatedTo(IArguments arguments);

    /// <summary>
    /// Executed when an element is navigated from.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    void OnNavigatedFrom(IArguments arguments);
}