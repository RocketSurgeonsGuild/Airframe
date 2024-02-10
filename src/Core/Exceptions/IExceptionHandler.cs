using System;

namespace Rocket.Surgery.Airframe.Exceptions;

/// <summary>
/// Interface representing an <see cref="Exception"/> <see cref="IObserver{T}"/>.
/// </summary>
public interface IExceptionHandler : IObserver<Exception>
{
}