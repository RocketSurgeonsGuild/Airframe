using System;
using System.Collections.Generic;
using System.Text;
using AppKit;
using DryIoc;
using Foundation;
using Splat.DryIoc;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Base <see cref="NSApplicationDelegate"/>.
    /// </summary>
    /// <seealso cref="UIKit.UIApplicationDelegate" />
    public abstract class ApplicationDelegateBase : NSApplicationDelegate
    {
        private IContainer _container;

        /// <summary>
        /// Gets or sets the dependency inversion container.
        /// </summary>
        public IContainer Container { get; protected set; }

        /// <inheritdoc/>
        public override void DidFinishLaunching(NSNotification notification)
        {
            base.DidFinishLaunching(notification);

            ComposeDelegate();
        }

        /// <inheritdoc/>
        public override void WillTerminate(NSNotification notification)
        {
            DecomposeDelegate();
            base.WillTerminate(notification);
        }

        /// <summary>
        /// Composes the <see cref="NSApplicationDelegate"/> and registers services.
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
        /// Decomposes the <see cref="NSApplicationDelegate"/> and registers services.
        /// </summary>
        protected virtual void DecomposeDelegate() => ((Container)Container)?.Dispose();

        /// <summary>
        /// Registers services with the <see cref="IContainer"/> instance.
        /// </summary>
        /// <param name="container">The container.</param>
        protected abstract void RegisterServices(IContainer container);
    }
}
