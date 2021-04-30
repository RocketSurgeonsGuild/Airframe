using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using Splat;

namespace Rocket.Surgery.Airframe.Forms
{
    /// <summary>
    /// Base ReactiveUI <see cref="ReactiveViewCell{TViewModel}"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveViewCell{TViewModel}" />
    [SuppressMessage("Microsoft.Usage", "CA2214:VirtualMemberCallInConstructor", Justification = "Consumers should be aware methods are for object construction.")]
    public abstract class ViewCellBase<TViewModel> : ReactiveViewCell<TViewModel>, IEnableLogger
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCellBase{TViewModel}"/> class.
        /// </summary>
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Consumers should be aware methods are for object construction.")]
        protected ViewCellBase()
        {
            Logger = this.Log();
            Initialize();
            BindControls();
            RegisterObservers();
        }

        /// <summary>
        /// Gets or sets the <see cref="IFullLogger"/>.
        /// </summary>
        public IFullLogger Logger { get; protected set; }

        /// <summary>
        /// Gets the view cell bindings.
        /// </summary>
        protected CompositeDisposable ViewCellBindings { get; } = new CompositeDisposable();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// View lifecycle method that initializes the view.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// View lifecycle method that sets up reactive bindings.
        /// </summary>
        protected virtual void BindControls()
        {
        }

        /// <summary>
        /// View lifecycle method that registers observers via subscriptions.
        /// </summary>
        protected virtual void RegisterObservers()
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ViewCellBindings?.Dispose();
            }
        }
    }
}
