namespace Airframe.Testing;

/// <summary>
/// Interface representing a thing™ that can notify.
/// </summary>
/// <typeparam name="T">The notification type.</typeparam>
public interface INotifier<T>
{
    /// <summary>
    /// Notifies an item to a subscriber.
    /// </summary>
    /// <param name="item">The item.</param>
    void Notify(T item);
}