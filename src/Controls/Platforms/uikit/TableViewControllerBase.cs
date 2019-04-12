using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;

using Foundation;
using ReactiveUI;
using UIKit;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base reactive <see cref="UITableViewController"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveTableViewController{TViewModel}" />
    public abstract class TableViewControllerBase<TViewModel> : ReactiveTableViewController<TViewModel>
        where TViewModel : class
    {
        private ISubject<Unit> _appeared;
        private ISubject<Unit> _disappeared;
        private ISubject<Unit> _isAppearing;
        private ISubject<Unit> _isDisappearing;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableViewControllerBase{TViewModel}"/> class.
        /// </summary>
        protected TableViewControllerBase()
        {
            Initialize();
        }

        /// <summary>
        /// Gets the subscription disposable.
        /// </summary>
        protected CompositeDisposable SubscriptionDisposables { get; } = new CompositeDisposable();

        /// <inheritdoc />
        public override void ViewWillAppear(bool animated)
        {
            _isAppearing.OnNext(Unit.Default);
            base.ViewWillAppear(animated);
        }

        /// <inheritdoc />
        public override void ViewWillDisappear(bool animated)
        {
            _isDisappearing.OnNext(Unit.Default);
            base.ViewWillDisappear(animated);
        }

        /// <inheritdoc />
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            _appeared.OnNext(Unit.Default);
        }

        /// <inheritdoc />
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _disappeared.OnNext(Unit.Default);
        }

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
            _isAppearing = new Subject<Unit>();
            _isDisappearing = new Subject<Unit>();
            _appeared = new Subject<Unit>();
            _disappeared = new Subject<Unit>();
            BindControls();
            SetupSubscriptions();
        }
    }
}