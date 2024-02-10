using System;
using System.Reactive.Disposables;

namespace Rocket.Surgery.Airframe;

/// <summary>
/// Represents a disposable base object.
/// </summary>
public abstract class DisposableBase : IDisposable
{
    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets the garbage disposables.
    /// </summary>
    protected CompositeDisposable Garbage { get; } = new CompositeDisposable();

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">Is disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Garbage.Dispose();
        }
    }
}