using System;
using System.Reactive;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Interface that represents an object that can connect and disconnect from as observable sequence.
/// </summary>
public interface IListener : IListener<Unit>;

/// <summary>
/// Interface that represents an object that can connect and disconnect from as observable sequence.
/// </summary>
/// <typeparam name="T">The type of object to listen for.</typeparam>
public interface IListener<out T>
{
    /// <summary>
    /// Start listening.
    /// </summary>
    /// <returns>A observable sequence of <see cref="T"/>.</returns>
    IObservable<T> Listen();

    /// <summary>
    /// Ignore the information.
    /// </summary>
    /// <returns>A completion notification.</returns>
    IObservable<Unit> Ignore();
}