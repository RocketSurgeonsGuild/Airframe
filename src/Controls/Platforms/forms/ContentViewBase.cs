using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace Rocket.Surgery.ReactiveUI
{
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
        }

        /// <summary>
        /// Gets the subscription disposable.
        /// </summary>
        protected CompositeDisposable SubscriptionDisposables { get; } = new CompositeDisposable();

        /// <summary>
        /// View lifecycle method that sets up reactive subscriptions.
        /// </summary>
        protected virtual void SetupSubscriptions()
        {
        }

        /// <summary>
        /// View lifecycle method that sets up reactive bindings.
        /// </summary>
        protected virtual void BindControls()
        {
        }

        private void Initialize()
        {
            BindControls();
            SetupSubscriptions();
        }
    }
}
