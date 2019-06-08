using System;
using System.Collections.Generic;
using System.Text;
using AppKit;
using ReactiveUI;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base <see cref="NSView"/>.
    /// </summary>
    /// <typeparam name="T">The view model type.</typeparam>
    public abstract class ViewBase<T> : ReactiveView<T>
        where T : class, IReactiveObject
    {
    }
}
