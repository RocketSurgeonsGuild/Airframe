using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe.Mediator;

/// <summary>
/// Handles a <see cref="TNotification"/>.
/// </summary>
/// <typeparam name="TNotification">The command type.</typeparam>
public interface INotificationHandler<TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handles the <see cref="TNotification"/>.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task Handle(TNotification command, CancellationToken cancellationToken);
}