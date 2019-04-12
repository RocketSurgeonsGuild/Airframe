using System.Reactive.Disposables;
using ReactiveUI;
using UIKit;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base reactive <see cref="UITableViewCell"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveTableViewCell{TViewModel}" />
    public abstract class TableViewCellBase<TViewModel> : ReactiveTableViewCell<TViewModel>
        where TViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableViewCellBase{TViewModel}"/> class.
        /// </summary>
        protected TableViewCellBase()
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