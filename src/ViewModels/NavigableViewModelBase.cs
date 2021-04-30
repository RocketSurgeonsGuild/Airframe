using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Sextant;

namespace Rocket.Surgery.Airframe.ViewModels
{
    /// <summary>
    /// Base view model for <see cref="Sextant"/> navigation.
    /// </summary>
    public abstract class NavigableViewModelBase : ViewModelBase, INavigable, IDestructible, IInitializable
    {
        private readonly ISubject<INavigationParameter> _navigatedTo = new Subject<INavigationParameter>();
        private readonly ISubject<INavigationParameter> _navigatedFrom = new Subject<INavigationParameter>();
        private readonly ISubject<INavigationParameter> _navigatingTo = new Subject<INavigationParameter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigableViewModelBase"/> class.
        /// </summary>
        protected NavigableViewModelBase() => Initialize = ReactiveCommand.CreateFromObservable(((IInitializable)this).Initialize);

        /// <inheritdoc />
        public virtual string Id { get; } = string.Empty;

        /// <summary>
        /// Gets the initialize command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> Initialize { get; }

        /// <summary>
        /// Gets the navigated to <see cref="INavigationParameter"/>.
        /// </summary>
        /// <returns>The parameter.</returns>
        protected IObservable<INavigationParameter> NavigatedTo => _navigatedTo.AsObservable();

        /// <summary>
        /// Gets the navigated from <see cref="INavigationParameter"/>.
        /// </summary>
        /// <returns>The parameter.</returns>
        protected IObservable<INavigationParameter> NavigatedFrom => _navigatedFrom.AsObservable();

        /// <summary>
        /// Gets the navigating to <see cref="INavigationParameter"/>.
        /// </summary>
        /// <returns>The parameter.</returns>
        protected IObservable<INavigationParameter> NavigatingTo => _navigatingTo.AsObservable();

        /// <inheritdoc/>
        IObservable<Unit> INavigated.WhenNavigatedTo(INavigationParameter navigationParameter) => Observable.Create<Unit>(observer =>
            {
                _navigatedTo.OnNext(navigationParameter);
                observer.OnNext(Unit.Default);
                observer.OnCompleted();
                return Disposable.Empty;
            });

        /// <inheritdoc/>
        IObservable<Unit> INavigated.WhenNavigatedFrom(INavigationParameter navigationParameter) => Observable.Create<Unit>(observer =>
        {
            _navigatedFrom.OnNext(navigationParameter);
            observer.OnNext(Unit.Default);
            observer.OnCompleted();
            return Disposable.Empty;
        });

        /// <inheritdoc/>
        IObservable<Unit> INavigating.WhenNavigatingTo(INavigationParameter navigationParameter) => Observable.Create<Unit>(observer =>
        {
            _navigatingTo.OnNext(navigationParameter);
            observer.OnNext(Unit.Default);
            observer.OnCompleted();
            return Disposable.Empty;
        });

        /// <inheritdoc/>
        void IDestructible.Destroy()
        {
            Destroy();
            Garbage.Dispose();
        }

        /// <inheritdoc/>
        IObservable<Unit> IInitializable.Initialize() => ExecuteInitialize();

        /// <summary>
        /// Template method for initializing the view model.
        /// </summary>
        /// <returns>A completion notification.</returns>
        protected virtual IObservable<Unit> ExecuteInitialize() => Observable.Return(Unit.Default);

        /// <summary>
        /// Template method to destroy the view model.
        /// </summary>
        protected virtual void Destroy()
        {
        }
    }
}