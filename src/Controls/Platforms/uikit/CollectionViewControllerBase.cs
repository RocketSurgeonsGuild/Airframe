using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using ReactiveUI;
using UIKit;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Base <see cref="ReactiveCollectionViewController"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveCollectionViewController{TViewModel}" />
    public abstract class CollectionViewControllerBase<TViewModel> : ReactiveCollectionViewController<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewControllerBase{TViewModel}"/> class.
        /// </summary>
        protected CollectionViewControllerBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewControllerBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="withLayout">The ui collection view layout.</param>
        protected CollectionViewControllerBase(UICollectionViewLayout withLayout) : base(withLayout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewControllerBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected CollectionViewControllerBase(IntPtr handle) : base(handle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewControllerBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected CollectionViewControllerBase(NSObjectFlag t) : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewControllerBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected CollectionViewControllerBase(NSCoder coder) : base(coder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewControllerBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="nibName">The name.</param>
        /// <param name="bundle">The bundle.</param>
        protected CollectionViewControllerBase(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }
    }
}
