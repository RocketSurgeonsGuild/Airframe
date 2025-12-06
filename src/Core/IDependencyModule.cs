namespace Rocket.Surgery.Airframe;

/// <summary>
/// Represents a dependency registration grouping for a container.
/// </summary>
/// <typeparam name="TContainer">The container type.</typeparam>
public interface IDependencyModule<TContainer>
{
    /// <summary>
    /// Register dependencies against the <see cref="TRegistrar"/>.
    /// </summary>
    /// <param name="registrar">The registrar.</param>
    /// <returns>The <see cref="TRegistrar"/>.</returns>
    TContainer Register(TContainer registrar);
}