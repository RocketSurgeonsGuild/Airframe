using System.Reactive;

namespace Rocket.Surgery.Airframe.Mediator;

/// <summary>
/// Represents a request that will return a specified <see cref="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public interface IRequest<TResult>;

/// <summary>
/// Represents a request that will return a completion.
/// </summary>
public interface IRequest : IRequest<Unit>;