using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Splat;
using Xamarin.Forms;

namespace Rocket.Surgery.ReactiveUI.Forms
{
    /// <summary>
    /// Base application abstraction.
    /// </summary>
    /// <seealso cref="Application" />
    [SuppressMessage("Microsoft.Usage", "CA2214:VirtualMemberCallInConstructor", Justification = "Consumers should be aware methods are for object construction.")]
    public abstract class ApplicationBase : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBase"/> class.
        /// </summary>
        protected ApplicationBase()
        {
            ComposeApplicationRoot();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBase" /> class.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected ApplicationBase(Action<IMutableDependencyResolver> configurator)
        {
            ComposeApplicationRoot(configurator);
        }

        /// <summary>
        /// Composes the applications composition root.
        /// </summary>
        protected virtual void ComposeApplicationRoot()
        {
        }

        /// <summary>
        /// Composes the applications composition root.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ComposeApplicationRoot(Action<IMutableDependencyResolver> configurator) => configurator(Locator.CurrentMutable);
    }
}
