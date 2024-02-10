using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe.Navigation;

/// <summary>
/// Represents a simple navigator.
/// </summary>
public interface INavigator
{
    /// <summary>
    /// Pushes a route onto the navigation.
    /// </summary>
    /// <param name="name">The route name.</param>
    /// <returns>A completion notification.</returns>
    Task Push(string name);

    /// <summary>
    /// Pushes a route onto the navigation.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <returns>A completion notification.</returns>
    Task Push<TViewModel>();

    /// <summary>
    /// Pushes a route onto the navigation.
    /// </summary>
    /// <param name="name">The route name.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>A completion notification.</returns>
    Task Push(string name, IArguments arguments);

    /// <summary>
    /// Pushes a route onto the navigation.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <returns>A completion notification.</returns>
    Task Push<TViewModel>(IArguments arguments);

    /// <summary>
    /// Pops a route off the navigation.
    /// </summary>
    /// <returns>A completion notification.</returns>
    Task Pop();

    /// <summary>
    /// Pops a route off the navigation.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <returns>A completion notification.</returns>
    Task Pop(IArguments arguments);
}