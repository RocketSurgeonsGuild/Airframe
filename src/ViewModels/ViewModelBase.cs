using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.ViewModels
{
    /// <summary>
    /// Base ReactiveUI View Model.
    /// </summary>
    [SuppressMessage("Microsoft.Usage",  "CA2214:VirtualMemberCallInConstructor", Justification = "Consumers should be aware methods are for object construction.")]
    public abstract class ViewModelBase : ReactiveObject, IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ViewModelBase()
        {
            ErrorInteraction = new Interaction<string, bool>();
            Subscriptions = new CompositeDisposable();

            Initialize();
            ComposeObservables();
            RegisterObservers();
        }

        /// <inheritdoc />
        public virtual string Id { get; }

        /// <inheritdoc />
        public virtual bool IsLoading { get; }

        /// <inheritdoc />
        public Interaction<string, bool> ErrorInteraction { get; }

        /// <summary>
        /// Gets the binding disposables.
        /// </summary>
        protected CompositeDisposable Subscriptions { get; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// View Model lifecycle method that composes observable pipelines.
        /// </summary>
        protected abstract void ComposeObservables();

        /// <summary>
        /// View Model lifecycle method that registers observers to pipelines.
        /// </summary>
        protected abstract void RegisterObservers();
    }
}