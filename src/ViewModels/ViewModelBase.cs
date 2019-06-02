using System.Reactive.Disposables;
using ReactiveUI;

namespace Rocket.Surgery.ReactiveUI
{
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
        }

        /// <inheritdoc />
        public Interaction<string, bool> AlertInteraction { get; }

        /// <inheritdoc />
        public Interaction<string, bool> ConfirmationInteraction { get; }

        /// <inheritdoc />
        public Interaction<string, bool> ErrorInteraction { get; }

        /// <inheritdoc />
        public virtual bool IsBusy { get; }

        /// <summary>
        /// Gets the binding disposables.
        /// </summary>
        protected CompositeDisposable Bindings { get; } = new CompositeDisposable();

        /// <summary>
        /// View Model lifecycle method that sets up observable pipelines.
        /// </summary>
        protected abstract void SetupObservables();

        /// <summary>
        /// View Model lifecycle method that registers observers to pipelines.
        /// </summary>
        protected abstract void RegisterObservers();
    }
}