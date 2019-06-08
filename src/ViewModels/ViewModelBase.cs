using System.Reactive.Disposables;
using ReactiveUI;

namespace Rocket.Surgery.ReactiveUI
{
#pragma warning disable CA2214 // Do not call overridable methods in constructors
    /// <summary>
    /// Base ReactiveUI View Model.
    /// </summary>
    public abstract class ViewModelBase : ReactiveObject, IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ViewModelBase()
        {
            AlertInteraction = new Interaction<string, bool>();
            ConfirmationInteraction = new Interaction<string, bool>();
            ErrorInteraction = new Interaction<string, bool>();
            ComposeObservables();
            RegisterObservers();
        }

        /// <inheritdoc />
        public Interaction<string, bool> AlertInteraction { get; }

        /// <inheritdoc />
        public Interaction<string, bool> ConfirmationInteraction { get; }

        /// <inheritdoc />
        public Interaction<string, bool> ErrorInteraction { get; }

        /// <inheritdoc />
        public virtual bool IsLoading { get; }

        /// <summary>
        /// Gets the binding disposables.
        /// </summary>
        protected CompositeDisposable Bindings { get; } = new CompositeDisposable();

        /// <summary>
        /// View Model lifecycle method that composes observable pipelines.
        /// </summary>
        protected abstract void ComposeObservables();

        /// <summary>
        /// View Model lifecycle method that registers observers to pipelines.
        /// </summary>
        protected abstract void RegisterObservers();
    }
#pragma warning restore CA2214 // Do not call overridable methods in constructors
}