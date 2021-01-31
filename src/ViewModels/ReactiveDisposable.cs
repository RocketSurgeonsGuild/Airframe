using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.ViewModels
{
    /// <summary>
    /// Represents a base object for garbage collection.
    /// </summary>
    public abstract class ReactiveDisposable : ReactiveObject, IDisposable
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