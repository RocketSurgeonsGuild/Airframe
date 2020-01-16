using System;
using System.Collections.Generic;
using System.Text;
using DryIoc;
using Foundation;
using Splat.DryIoc;
using UIKit;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Base application delegate implementation.
    /// </summary>
    public abstract class ApplicationDelegateBase : UIApplicationDelegate
    {
        private IContainer _container;

        /// <summary>
        /// Gets or sets the dependency inversion container.
        /// </summary>
        public IContainer Container { get; protected set; }

        /// <inheritdoc/>
        public override void FinishedLaunching(UIApplication application)
        {
            base.FinishedLaunching(application);

            ComposeDelegate();
        }

        /// <inheritdoc/>
        public override bool FinishedLaunching(UIApplication application, Foundation.NSDictionary launchOptions)
        {
            ComposeDelegate();

            return true;
        }

        /// <summary>
        /// Composes the <see cref="UIApplicationDelegate"/> and registers services.
        /// </summary>
        protected virtual void ComposeDelegate()
        {
            _container = CreateContainer();
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
        /// Registers services with the <see cref="IContainer"/> instance.
        /// </summary>
        /// <param name="container">The container.</param>
        protected abstract void RegisterServices(IContainer container);
    }
}
