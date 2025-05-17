namespace Rocket.Surgery.Airframe.Mediator;

/// <summary>
/// Represents a query made to the mediator.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public interface IQuery<TResult> : IRequest<TResult>;