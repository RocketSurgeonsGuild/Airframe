using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe.Mediator;

/// <summary>
/// Handles a <see cref="TCommand"/>.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the <see cref="TCommand"/>.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task Handle(TCommand command, CancellationToken cancellationToken);
}