using System.Reactive.Disposables;
using ReactiveUI;
using UIKit;

namespace Rocket.Surgery.ReactiveUI.Controls
{
    /// <summary>
    /// Base reactive <see cref="UITableView"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveTableView{TViewModel}" />
    public class TableViewBase<TViewModel> : ReactiveTableView<TViewModel>
        where TViewModel : class
    {
        /// <summary>
        /// Gets the subscription disposable.
        /// </summary>
        protected CompositeDisposable SubscriptionDisposable { get; } = new CompositeDisposable();

        /// <summary>
        /// View lifecycle method that sets up reactive subscriptions.
        /// </summary>
        protected virtual void SetupReactiveSubscriptions()
        {
        }

        /// <summary>
        /// View lifecycle method that sets up reactive bindings.
        /// </summary>
        protected virtual void SetupReactiveBindings()
        {
        }
    }
}