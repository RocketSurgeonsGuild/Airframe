using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Airframe;

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

public abstract class Listener<T> : DisposableBase, IListener<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Listener{T}"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    protected Listener(ILoggerFactory loggerFactory)
    {
        _stop.DisposeWith(Garbage);
        Logger = loggerFactory.CreateLogger(GetType());
        Listening = Observable.Empty<T>().Publish();
    }

    /// <inheritdoc />
    IObservable<T> IListener<T>.Listen() => StartListening().LogTrace(Logger, nameof(Listen));

    /// <inheritdoc cref="IListener"/>
    IObservable<Unit> IListener<T>.Ignore() => StopListening().LogTrace(Logger, nameof(Disconnect));

    protected virtual IConnectableObservable<T> Listen() => Listening;

    /// <summary>
    /// Disconnects from the observable sequence.
    /// </summary>
    /// <returns></returns>
    protected virtual IObservable<Unit> Disconnect() => Observable.Return(Unit.Default);

    protected IConnectableObservable<T> Listening { get; set; }

    protected ILogger Logger { get; }

    private IObservable<T> StartListening() => Observable.Create<T>(observer =>
        {
            _stop = Listen().Connect();

            return Listen().Subscribe(observer);
        }
    );

    private IObservable<Unit> StopListening() => Disconnect().Finally(() => _stop.Dispose());

    private IDisposable _stop = Disposable.Empty;
}