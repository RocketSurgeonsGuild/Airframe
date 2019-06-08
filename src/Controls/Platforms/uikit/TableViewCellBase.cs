using System;
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
        /// Initializes a new instance of the <see cref="TableViewCellBase{TViewModel}" /> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected TableViewCellBase(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Gets the control binding disposables.
        /// </summary>
        protected CompositeDisposable ControlBindings { get; } = new CompositeDisposable();

        /// <inheritdoc />
        public override void UpdateConstraints()
        {
            SetupCellConstraints();

            base.UpdateConstraints();
        }

        /// <summary>
        /// View lifecycle method that registers observers via subscriptions.
        /// </summary>
        protected abstract void RegisterObservers();

        /// <summary>
        /// View lifecycle method that sets up reactive bindings.
        /// </summary>
        protected abstract void BindControls();

        /// <summary>
        /// Creates the cell interface.
        /// </summary>
        protected abstract void CreateCellInterface();

        /// <summary>
        /// Setups the cell constraints.
        /// </summary>
        protected abstract void SetupCellConstraints();

        private void Initialize()
        {
            CreateCellInterface();
            BindControls();
            RegisterObservers();
            SetNeedsUpdateConstraints();
        }
    }
}