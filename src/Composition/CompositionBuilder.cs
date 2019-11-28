using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
using ReactiveUI;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Builds a composition root.
    /// </summary>
    public sealed class CompositionBuilder
    {
        private readonly ContainerBuilder _containerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionBuilder"/> class.
        /// </summary>
        public CompositionBuilder()
        {
            _containerBuilder = new ContainerBuilder();
        }

        /// <summary>
        /// Loads the autofac module.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <returns>The composition builder.</returns>
        public CompositionBuilder RegisterModule<TModule>()
            where TModule : IModule, new()
        {
            _containerBuilder.RegisterModule<TModule>();
            return this;
        }

        /// <summary>
        /// Loads the autofac module.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <param name="module">The module.</param>
        /// <returns>The composition builder.</returns>
        public CompositionBuilder RegisterModule<TModule>(TModule module)
            where TModule : IModule
        {
            _containerBuilder.RegisterModule(module);
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
        {
            _containerBuilder.RegisterType<TClass>().As<TInterface>().SingleInstance();
            return this;
        }

        /// <summary>
        /// Registers Views to View Models.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The composition builder.</returns>
        public CompositionBuilder RegisterView<TView, TViewModel>()
            where TView : IViewFor
            where TViewModel : class
        {
            _containerBuilder.RegisterType<TView>().As<IViewFor<TViewModel>>();
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
            _containerBuilder.RegisterType<T>().AsSelf();
            return this;
        }

        /// <summary>
        /// Compose the <see cref="IContainer"/>.
        /// </summary>
        /// <returns>The container.</returns>
        public IContainer Compose() => _containerBuilder.Build();
    }
}
