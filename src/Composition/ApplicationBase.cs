using System.Diagnostics.CodeAnalysis;
using DryIoc;
using Splat.DryIoc;
using Xamarin.Forms;

namespace Rocket.Surgery.Airframe.Composition
{
    /// <summary>
    /// Base application abstraction.
    /// </summary>
    /// <seealso cref="Application" />
    [SuppressMessage("Microsoft.Usage", "CA2214:VirtualMemberCallInConstructor", Justification = "Consumers should be aware methods are for object construction.")]
    public abstract class ApplicationBase : Application
    {
        private readonly IPlatformRegistrar? _platformRegistrar;
        private IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBase"/> class.
        /// </summary>
        protected ApplicationBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBase" /> class.
        /// </summary>
        /// <param name="platformRegistrar">The platformRegistrar.</param>
        [SuppressMessage("ReSharper", "CS8618:NonNullableMember", Justification = "Consumers should be aware methods are for object construction.")]
        protected ApplicationBase(IPlatformRegistrar? platformRegistrar)
        {
            _platformRegistrar = platformRegistrar;
            Initialize();
        }

        /// <summary>
        /// Gets the current application container.
        /// </summary>
        public static new ApplicationBase Current => (ApplicationBase)Application.Current;

        /// <summary>
        /// Gets or sets the dependency inversion container.
        /// </summary>
        public IContainer Container { get; protected set; }

        /// <summary>
        /// Initialize the <see cref="ApplicationBase"/>.
        /// </summary>
        protected void Initialize() => ComposeApplicationRoot();

        /// <summary>
        /// Composes the applications composition root.
        /// </summary>
        protected virtual void ComposeApplicationRoot()
        {
            _container = CreateContainer();
            _platformRegistrar?.RegisterPlatformServices(_container);
            RegisterServices(_container);
            _container.UseDryIocDependencyResolver();
            Container = _container.WithNoMoreRegistrationAllowed();
        }

        /// <summary>
        /// Returns a new container instance.
        /// </summary>
        /// <returns>The container.</returns>
        protected virtual IContainer CreateContainer() => new Container();

        /// <summary>
        /// Register the platform services.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        protected virtual void RegisterPlatformServices(IPlatformRegistrar registrar) =>
            registrar?.RegisterPlatformServices(_container);

        /// <summary>
        /// Registers services with the <see cref="IContainer"/> instance.
        /// </summary>
        /// <param name="container">The container.</param>
        protected abstract void RegisterServices(IContainer container);
    }
}
