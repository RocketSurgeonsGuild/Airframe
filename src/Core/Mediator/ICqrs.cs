namespace Rocket.Surgery.Airframe.Mediator;

/// <summary>
/// Interface representing a mediator implementation.
/// </summary>
public interface ICqrs : IExecutor, ISender, IPublisher;