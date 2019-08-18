using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base <see cref="ReactiveView"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    public abstract class ViewBase<TViewModel> : ReactiveView<TViewModel>
        where TViewModel : class, IReactiveObject
    {
    }
}
