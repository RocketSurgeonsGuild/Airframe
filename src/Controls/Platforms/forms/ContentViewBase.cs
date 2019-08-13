using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace Rocket.Surgery.ReactiveUI
{
#pragma warning disable CA2214 // Do not call overridable methods in constructors
    /// <summary>
    /// Base ReactiveUI <see cref="ReactiveContentView{TViewModel}"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveContentView{TViewModel}" />
    public abstract class ContentViewBase<TViewModel> : ReactiveContentView<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentViewBase{TViewModel}"/> class.
        /// </summary>
        protected ContentViewBase()
        {
            Initialize();
            BindControls();
            RegisterObservers();
        }

        /// <summary>
        /// Gets the subscription disposable.
        /// </summary>
        protected CompositeDisposable ViewBindings { get; } = new CompositeDisposable();

        /// <summary>
        /// View lifecycle method that registers observers via subscriptions.
        /// </summary>
        protected virtual void RegisterObservers()
        {
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
#pragma warning restore CA2214 // Do not call overridable methods in constructors
    }
}
