using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using ReactiveUI;
using UIKit;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Base reactive <see cref="UITableViewCell"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveTableViewCell{TViewModel}" />
    [SuppressMessage("Microsoft.Usage",  "CA2214:VirtualMemberCallInConstructor", Justification = "Consumers should be aware methods are for object construction.")]
    public abstract class TableViewCellBase<TViewModel> : ReactiveTableViewCell<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableViewCellBase{TViewModel}" /> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected TableViewCellBase(IntPtr handle)
            : base(handle)
        {
            Initialize();
            CreateCellInterface();
            BindControls();
            RegisterObservers();
            SetNeedsUpdateConstraints();
        }

        /// <summary>
        /// Gets the control binding disposables.
        /// </summary>
        protected CompositeDisposable ViewBindings { get; } = new CompositeDisposable();

        /// <inheritdoc />
        public override void UpdateConstraints()
        {
            SetupCellConstraints();

            base.UpdateConstraints();
        }

        /// <summary>
        /// View lifecycle method that initializes the view controller.
        /// </summary>
        protected virtual void Initialize()
        {
        }

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

        /// <summary>
        /// View lifecycle method that registers observers via subscriptions.
        /// </summary>
        protected abstract void RegisterObservers();
    }
}