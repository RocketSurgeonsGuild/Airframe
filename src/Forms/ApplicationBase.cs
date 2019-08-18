using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xamarin.Forms;

namespace Rocket.Surgery.ReactiveUI.Forms
{
    /// <summary>
    /// Base application abstraction.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Application" />
    [SuppressMessage("Microsoft.Usage", "CA2214", Justification = "Justification", MessageId = "MessageId", Scope = "Scope", Target = "Target")]
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
        /// Gets the current application.
        /// </summary>
        public static new ApplicationBase Current => Application.Current as ApplicationBase;

        /// <summary>
        /// Composes the applications composition root.
        /// </summary>
        protected abstract void ComposeApplicationRoot();
    }
}
