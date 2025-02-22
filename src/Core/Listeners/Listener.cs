using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Base extension point for an <see cref="IListener"/>.
/// </summary>
[PublicAPI]
public abstract class Listener : Listener<Unit>, IListener
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Listener"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    protected Listener(ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
    }
}

/// <summary>
/// Base extension point for an <see cref="IListener{T}"/>.
/// </summary>
/// <typeparam name="T">The observable sequence type.</typeparam>
[PublicAPI]
public abstract class Listener<T> : DisposableBase, IListener<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Listener{T}"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    protected Listener(ILoggerFactory loggerFactory)
    {
        _stop = Disposable.Empty;
        _stop.DisposeWith(Garbage);
        Logger = loggerFactory.CreateLogger(GetType());
        Listening = Observable.Empty<T>().Publish();
    }

    /// <inheritdoc />
    IObservable<T> IListener<T>.Listen() => StartListening();

    /// <inheritdoc cref="IListener"/>
    IObservable<Unit> IListener<T>.Ignore() => StopListening();

    /// <summary>
    /// Connect to the observable sequence.
    /// </summary>
    /// <returns>Returns a signal.</returns>
    protected virtual IConnectableObservable<T> Listen() => Listening;

    /// <summary>
    /// Disconnects from the observable sequence.
    /// </summary>
    /// <returns>Returns a signal.</returns>
    protected virtual IObservable<Unit> Ignore() => Observable.Return(Unit.Default);

    /// <summary>
    /// Gets or sets the <see cref="IConnectableObservable{T}"/> that the listener listens to.
    /// </summary>
    protected IConnectableObservable<T> Listening { get; set; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger Logger { get; }

    private IObservable<T> StartListening() => Observable.Create<T>(
        observer =>
        {
            _stop = Listen().Connect();

            return Listen().Subscribe(observer);
        });

    private IObservable<Unit> StopListening() => Ignore().Finally(() => _stop.Dispose());

    private IDisposable _stop;
}