namespace Rocket.Surgery.Airframe;

/// <summary>
/// Represents a dependency registration grouping for a container.
/// </summary>
/// <typeparam name="TContainer">The container type.</typeparam>
public interface IDependencyModule<TContainer>
{
    /// <summary>
    /// Register dependencies against the <see cref="TContainer"/>.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <returns>The <see cref="TContainer"/>.</returns>
    TContainer Register(TContainer container);
}