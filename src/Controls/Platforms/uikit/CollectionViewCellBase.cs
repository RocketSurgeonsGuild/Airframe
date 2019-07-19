using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base UICollectionView.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveCollectionViewCell{TViewModel}" />
    public abstract class CollectionViewCellBase<TViewModel> : ReactiveCollectionViewCell<TViewModel>
        where TViewModel : class, IReactiveObject
    {
    }
}
