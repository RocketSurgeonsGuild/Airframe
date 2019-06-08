using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base <see cref="ReactiveImageView"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveUI.ReactiveImageView{TViewModel}" />
    public abstract class ImageViewBase<TViewModel> : ReactiveImageView<TViewModel>
        where TViewModel : class, IReactiveObject
    {
    }
}
