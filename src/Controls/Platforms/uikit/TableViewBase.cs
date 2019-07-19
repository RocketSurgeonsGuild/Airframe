using System.Reactive.Disposables;
using ReactiveUI;
using UIKit;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base reactive <see cref="UITableView"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveTableView{TViewModel}" />
    public abstract class TableViewBase<TViewModel> : ReactiveTableView<TViewModel>
        where TViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableViewBase{TViewModel}"/> class.
        /// </summary>
        protected TableViewBase()
        {
            Initialize();
        }

        /// <summary>
        /// Gets the control bindings disposable.
        /// </summary>
        protected CompositeDisposable ControlBindings { get; } = new CompositeDisposable();


        /// <summary>
        /// View lifecycle method that registers observers via subscriptions.
        /// </summary>
        protected abstract void RegisterObservers();

        /// <summary>
        /// View lifecycle method that sets up reactive bindings.
        /// </summary>
        protected abstract void BindControls();

        private void Initialize()
        {
            BindControls();
            RegisterObservers();
        }
    }
}