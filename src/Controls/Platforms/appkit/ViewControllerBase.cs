using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Foundation;
using ReactiveUI;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Base <see cref="ReactiveViewController" />.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveViewController{TViewModel}" />
    /// <seealso cref="ReactiveViewController" />
    public abstract class ViewControllerBase<TViewModel> : ReactiveViewController<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        private readonly ISubject<Unit> _appeared;
        private readonly ISubject<Unit> _disappeared;
        private readonly ISubject<Unit> _appearing;
        private readonly ISubject<Unit> _disappearing;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewControllerBase{TViewModel}"/> class.
        /// </summary>
        protected ViewControllerBase()
        {
            _appearing = new Subject<Unit>();
            _disappearing = new Subject<Unit>();
            _appeared = new Subject<Unit>();
            _disappeared = new Subject<Unit>();
        }

        /// <summary>
        /// Gets the control binding disposable.
        /// </summary>
        protected CompositeDisposable ViewBindings { get; } = new CompositeDisposable();

        /// <summary>
        /// Gets an observable sequence when the view is appearing.
        /// </summary>
        /// <returns>The appearing notification.</returns>
        public virtual IObservable<Unit> Appearing() => _appearing.AsObservable();

        /// <summary>
        /// Gets an observable sequence when the view is disappearing.
        /// </summary>
        /// <returns>The appearing notification.</returns>
        public virtual IObservable<Unit> Appeared() => _appeared.AsObservable();

        /// <summary>
        /// Gets an observable sequence when the view is appearing.
        /// </summary>
        /// <returns>The appearing notification.</returns>
        public virtual IObservable<Unit> Disappeared() => _disappeared.AsObservable();

        /// <summary>
        /// Gets an observable sequence when the view is disappearing.
        /// </summary>
        /// <returns>The appearing notification.</returns>
        public virtual IObservable<Unit> IsDisappearing() => _appearing.AsObservable();

        /// <inheritdoc />
        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            _appearing.OnNext(Unit.Default);
        }

        /// <inheritdoc />
        public override void ViewWillDisappear()
        {
            base.ViewWillDisappear();
            _disappearing.OnNext(Unit.Default);
        }

        /// <inheritdoc />
        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
            _appeared.OnNext(Unit.Default);
        }

        /// <inheritdoc />
        public override void ViewDidDisappear()
        {
            base.ViewDidDisappear();
            _disappeared.OnNext(Unit.Default);
        }

        /// <inheritdoc/>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Initialize();
            CreateUserInterface();
            BindControls();
            RegisterObservers();
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
        /// View lifecycle method that creates the user interface.
        /// </summary>
        protected abstract void CreateUserInterface();

        /// <summary>
        /// View lifecycle method that registers observers via subscriptions.
        /// </summary>
        protected abstract void RegisterObservers();
    }
}
