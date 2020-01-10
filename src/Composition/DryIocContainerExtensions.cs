using DryIoc;
using ReactiveUI;

namespace Rocket.Surgery.Airframe.Composition
{
    /// <summary>
    /// <see cref="IRegistrator"/> extensions methods.
    /// </summary>
    public static class DryIocContainerExtensions
    {
        /// <summary>
        /// Registers a module to the <see cref="IContainer"/> instance.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <typeparam name="TModule">The module type.</typeparam>
        /// <returns>The container instance.</returns>
        public static IContainer RegisterModule<TModule>(this IContainer container)
            where TModule : IModule
        {
            container.Register<TModule>();
            var module = container.Resolve<TModule>();
            module.Load(container);
            return container;
        }

        /// <summary>
        /// Registers a module to the <see cref="IRegistrator"/> instance.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="module">The module instance.</param>
        /// <typeparam name="TModule">The module type.</typeparam>
        /// <returns>The registrar instance.</returns>
        public static IRegistrator RegisterModule<TModule>(this IRegistrator registrar, TModule module)
            where TModule : IModule
        {
            module.Load(registrar);
            return registrar;
        }

        /// <summary>
        /// Registers Views to View Models.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The composition builder.</returns>
        public static IRegistrator RegisterView<TView, TViewModel>(this IRegistrator registrar)
            where TView : IViewFor<TViewModel>
            where TViewModel : class
        {
            registrar.Register<IViewFor<TViewModel>, TView>();
            return registrar;
        }

        /// <summary>
        /// Registers Views to View Models.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <returns>The composition builder.</returns>
        public static IRegistrator RegisterViewModel<T>(this IRegistrator registrar)
            where T : class
        {
            registrar.Register<T>();
            return registrar;
        }
    }
}