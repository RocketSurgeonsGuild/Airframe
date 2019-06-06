using System;
using System.Linq;
using System.Reflection;
using Autofac;
using ReactiveUI;
using UIKit;

namespace Composition
{
    /// <summary>
    /// <see cref="UIApplicationDelegate"/> abstraction that provides hooks to build the composition root.
    /// </summary>
    /// <seealso cref="UIKit.UIApplicationDelegate" />
    public abstract class ApplicationDelegateBase : UIApplicationDelegate
    {
        /// <inheritdoc />
        public override void FinishedLaunching(UIApplication application)
        {
            base.FinishedLaunching(application);

            ComposeRoot();
        }

        /// <inheritdoc />
        public override bool FinishedLaunching(UIApplication application, Foundation.NSDictionary launchOptions)
        {
            ComposeRoot();

            return true;
        }

        /// <summary>
        /// Builds the application composition root.
        /// </summary>
        protected abstract void ComposeRoot();
    }
}
