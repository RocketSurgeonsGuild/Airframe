using System;
using System.Reactive.Disposables;
using ReactiveMarbles.Mvvm;

namespace Rocket.Surgery.Airframe.ViewModels;

/// <summary>
/// Base ReactiveUI View Model.
/// </summary>
public abstract class ViewModelBase : RxObject, IDisposable
{
    /// <summary>
    /// Gets or sets a value indicating whether this view model is loading.
    /// </summary>
    public bool IsLoading { get; protected set; }

    /// <summary>
    /// Gets the collection of disposables.
    /// </summary>
    protected CompositeDisposable Garbage { get; } = new CompositeDisposable();

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the garbage.
    /// </summary>
    /// <param name="disposing">Whether the object is disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Garbage.Dispose();
        }
    }
}