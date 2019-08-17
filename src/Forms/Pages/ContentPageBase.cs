using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace Rocket.Surgery.ReactiveUI.Forms
{
#pragma warning disable CA2214 // Do not call overridable methods in constructors
    /// <summary>
    /// Base ReactiveUI <see cref="ReactiveContentPage{TViewModel}"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveContentPage{TViewModel}" />
    public abstract class ContentPageBase<TViewModel> : ReactiveContentPage<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        private readonly ISubject<Unit> _isAppearing;
        private readonly ISubject<Unit> _isDisappearing;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPageBase{TViewModel}"/> class.
        /// </summary>
        protected ContentPageBase()
        {
            _isAppearing = new Subject<Unit>();
            _isDisappearing = new Subject<Unit>();
            Initialize();
            BindControls();
            RegisterObservers();
        }

        /// <summary>
        /// Gets the subscription disposable.
        /// </summary>
        protected CompositeDisposable ViewBindings { get; } = new CompositeDisposable();

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
