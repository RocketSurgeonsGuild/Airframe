using DryIoc;
using JetBrains.Annotations;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.Composition
{
    /// <summary>
    /// Builds a composition root.
    /// </summary>
    public sealed class CompositionBuilder
    {
        private readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionBuilder"/> class.
        /// </summary>
        /// <param name="container">The container instance.</param>
        public CompositionBuilder(IContainer? container = null) =>
            _container = container ?? new Container();

        /// <summary>
        /// Loads the autofac module.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <returns>The composition builder.</returns>
        public CompositionBuilder RegisterModule<TModule>()
            where TModule : IModule, new()
        {
            _container.RegisterModule<TModule>();
            return this;
        }

        /// <summary>
        /// Loads the autofac module.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <param name="module">The module.</param>
        /// <returns>The composition builder.</returns>
        public CompositionBuilder RegisterModule<TModule>([NotNull] TModule module)
            where TModule : IModule
        {
            _container.RegisterModule(module);
            return this;
        }

        /// <summary>
        /// Registers custom command binders.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <returns>
        /// The composition builder.
        /// </returns>
        public CompositionBuilder RegisterCommandBinders<TInterface, TClass>()
            where TClass : class, TInterface
        {
            _container.Register<TInterface, TClass>(Reuse.Singleton);
            return this;
        }

        /// <summary>
        /// Registers Views to View Models.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The composition builder.</returns>
        public CompositionBuilder RegisterView<TView, TViewModel>()
            where TView : IViewFor<TViewModel>
            where TViewModel : class
        {
            _container.Register<IViewFor<TViewModel>, TView>();
            return this;
        }

        /// <summary>
        /// Registers Views to View Models.
        /// </summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <returns>The composition builder.</returns>
        public CompositionBuilder RegisterViewModel<T>()
            where T : class
        {
            _container.Register<T>();
            return this;
        }

        /// <summary>
        /// Compose the <see cref="IContainer"/>.
        /// </summary>
        /// <returns>The container.</returns>
        public IContainer Compose() => _container.WithNoMoreRegistrationAllowed();

        /// <summary>
        /// Compose the <see cref="IContainer"/>.
        /// </summary>
        /// <param name="ignore">Ignore instead of throw.</param>
        /// <returns>The container.</returns>
        public IContainer Compose(bool ignore) => _container.WithNoMoreRegistrationAllowed(ignore);
    }
}
