using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base <see cref="ReactiveControl" />.
    /// </summary>
    /// <typeparam name="TVieModel">The view model type.</typeparam>
    public abstract class ControlBase<TVieModel> : ReactiveControl<TVieModel>
        where TVieModel : class, IReactiveObject
    {
    }
}
