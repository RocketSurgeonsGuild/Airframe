using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using UIKit;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Base application delegate implementation.
    /// </summary>
    public abstract class ApplicationDelegateBase : UIApplicationDelegate
    {
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
        protected abstract void ComposeDelegate();
    }
}
