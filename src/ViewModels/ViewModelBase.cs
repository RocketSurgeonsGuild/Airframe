using System;
using System.Reactive.Disposables;

namespace Rocket.Surgery.Airframe.ViewModels
{
    /// <summary>
    /// Base ReactiveUI View Model.
    /// </summary>
    public abstract class ViewModelBase : IDisposable
    {
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
}