using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Sextant;
using Sextant.Plugins.Popup;
using Xamarin.Forms;

[assembly: XmlnsPrefix("https://schemas.rocketsurgeonsguild.com/xaml/airframe/popup", "popup")]
[assembly: XmlnsDefinition("https://schemas.rocketsurgeonsguild.com/xaml/airframe/popup", "Rocket.Surgery.Airframe.Popup")]

[assembly: SuppressMessage("Microsoft.Usage",  "CA2214:VirtualMemberCallInConstructor", Justification = "Consumers should be aware methods are for object construction.")]
namespace Rocket.Surgery.Airframe.Popup
{
    /// <summary>
    /// Base ReactiveUI <see cref="SextantPopupPage{TViewModel}"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="SextantPopupPage{TViewModel}" />
    public abstract class PopupPageBase<TViewModel> : SextantPopupPage<TViewModel>, IDisposable
        where TViewModel : class, IReactiveObject, IViewModel
    {
        private readonly ISubject<Unit> _isAppearing;
        private readonly ISubject<Unit> _isDisappearing;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupPageBase{TViewModel}"/> class.
        /// </summary>
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Consumers should be aware methods are for object construction.")]
        protected PopupPageBase()
        {
            _isAppearing = new Subject<Unit>();
            _isDisappearing = new Subject<Unit>();
            Initialize();
            BindControls();
            RegisterObservers();
        }

        /// <summary>
        /// Gets the view bindings disposable.
        /// </summary>
        protected CompositeDisposable ViewBindings { get; } = new CompositeDisposable();

        /// <summary>
        /// Gets an observable sequence when the view is appearing.
        /// </summary>
        /// <returns>The appearing notification.</returns>
        protected virtual IObservable<Unit> IsAppearing => _isAppearing.AsObservable();

        /// <summary>
        /// Gets an observable sequence when the view is disappearing.
        /// </summary>
        /// <returns>The appearing notification.</returns>
        protected virtual IObservable<Unit> IsDisappearing => _isDisappearing.AsObservable();

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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

        /// <summary>
        /// View lifecycle method that registers observers via subscriptions.
        /// </summary>
        protected virtual void RegisterObservers()
        {
        }

        /// <summary>
        /// Disposes of the resources allocated for this instance.
        /// </summary>
        /// <param name="disposing">A value indicating whether or not the instance is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ViewBindings.Dispose();
            }
        }
    }
}