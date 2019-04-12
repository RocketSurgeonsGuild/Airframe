using System.Reactive.Disposables;
using ReactiveUI;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base ReactiveUI View Model.
    /// </summary>
    public abstract class ViewModelBase : ReactiveObject, IViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ViewModelBase()
        {
            AlertInteraction = new Interaction<string, bool>();
            ConfirmationInteraction = new Interaction<string, bool>();
            ErrorInteraction = new Interaction<string, bool>();
        }

        /// <inheritdoc />
        public Interaction<string, bool> AlertInteraction { get; }

        /// <inheritdoc />
        public Interaction<string, bool> ConfirmationInteraction { get; }

        /// <inheritdoc />
        public Interaction<string, bool> ErrorInteraction { get; }

        /// <inheritdoc />
        public virtual bool IsBusy { get; }

        /// <summary>
        /// Gets the subscription disposables.
        /// </summary>
        protected CompositeDisposable SubscriptionDisposables { get; } = new CompositeDisposable();

        /// <summary>
        /// View Model lifecycle method that sets up reactive observables.
        /// </summary>
        protected virtual void SetupObservables()
        {
        }

        /// <summary>
        /// View Model lifecycle method that sets up reactive subscriptions.
        /// </summary>
        protected virtual void SetupSubscriptions()
        {
        }
    }
}