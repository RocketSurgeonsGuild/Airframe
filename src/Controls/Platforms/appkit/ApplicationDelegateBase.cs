using System;
using System.Collections.Generic;
using System.Text;
using AppKit;
using Foundation;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Base <see cref="NSApplicationDelegate"/>.
    /// </summary>
    /// <seealso cref="UIKit.UIApplicationDelegate" />
    public abstract class ApplicationDelegateBase : NSApplicationDelegate
    {
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
        protected abstract void ComposeDelegate();

        /// <summary>
        /// Decomposes the <see cref="NSApplicationDelegate"/> and registers services.
        /// </summary>
        protected abstract void DecomposeDelegate();
    }
}
