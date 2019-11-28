using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Text;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Rocket.Surgery.Airframe.Forms
{
    /// <summary>
    /// Base ReactiveUI <see cref="ReactiveViewCell{TViewModel}"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveViewCell{TViewModel}" />
    [SuppressMessage("Microsoft.Usage", "CA2214:VirtualMemberCallInConstructor", Justification = "Consumers should be aware methods are for object construction.")]
    public abstract class ViewCellBase<TViewModel> : ReactiveViewCell<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCellBase{TViewModel}"/> class.
        /// </summary>
        protected ViewCellBase()
        {
            Initialize();
            BindControls();
            RegisterObservers();
        }

        /// <summary>
        /// Gets the view cell bindings.
        /// </summary>
        protected CompositeDisposable ViewCellBindings { get; } = new CompositeDisposable();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// View lifecycle method that sets up reactive bindings.
        /// </summary>
        protected virtual void BindControls()
        {
        }

        /// <summary>
        /// View lifecycle method that initializes the view.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// View lifecycle method that registers observers via subscriptions.
        /// </summary>
        protected virtual void RegisterObservers()
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ViewCellBindings?.Dispose();
            }
        }
    }
}
