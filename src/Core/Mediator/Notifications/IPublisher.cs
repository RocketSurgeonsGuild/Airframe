using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe.Mediator;

/// <summary>
/// Represents an abstraction that publishes an event to multiple handlers.
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// Publish the provided <see cref="TNotification"/>.
    /// </summary>
    /// <param name="notification">The notification.</param>
    /// <typeparam name="TNotification">The notification type.</typeparam>
    /// <returns>A completion.</returns>
    Task Publish<TNotification>(TNotification notification)
        where TNotification : INotification;
}