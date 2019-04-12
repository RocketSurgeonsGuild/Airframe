using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base ReactiveUI <see cref="ReactiveContentPage{TViewModel}"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveContentPage{TViewModel}" />
    public abstract class ContentPageBase<TViewModel> : ReactiveContentPage<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        private ISubject<Unit> _isAppearing;
        private ISubject<Unit> _isDisappearing;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPageBase{TViewModel}"/> class.
        /// </summary>
        protected ContentPageBase()
        {
            Initialize();
        }

        /// <summary>
        /// Gets the subscription disposable.
        /// </summary>
        protected CompositeDisposable SubscriptionDisposables { get; } = new CompositeDisposable();

        /// <summary>
        /// Gets an observable sequence when the view is appearing.
        /// </summary>
        /// <returns>The appearing notification.</returns>
        public virtual IObservable<Unit> IsAppearing() => _isAppearing;

        /// <summary>
        /// Gets an observable sequence when the view is disappearing.
        /// </summary>
        /// <returns>The appearing notification.</returns>
        public virtual IObservable<Unit> IsDisappearing() => _isAppearing;

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            _isAppearing.OnNext(Unit.Default);
            base.OnAppearing();
        }

        /// <inheritdoc />
        protected override void OnDisappearing()
        {
            _isDisappearing.OnNext(Unit.Default);
            base.OnDisappearing();
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
            BindControls();
            SetupSubscriptions();
        }
    }
}
