namespace Rocket.Surgery.Airframe.Data;

/// <summary>
/// Interface representing an entity with an identifier.
/// </summary>
/// <typeparam name="T">The identifier type.</typeparam>
public interface IHaveIdentifier<T>
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    T Id { get; set; }
}