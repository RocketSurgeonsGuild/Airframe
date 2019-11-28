using System;
using System.Collections.Generic;
using System.Text;
using CoreGraphics;
using Foundation;
using ReactiveUI;
using UIKit;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Base UICollectionView.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <seealso cref="ReactiveCollectionView{TViewModel}" />
    public abstract class CollectionViewBase<TViewModel> : ReactiveCollectionView<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewBase{T}"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected CollectionViewBase(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewBase{T}"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected CollectionViewBase(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewBase{T}"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected CollectionViewBase(NSCoder coder)
            : base(coder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewBase{T}"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="layout">The ui collection view layout.</param>
        protected CollectionViewBase(CGRect frame, UICollectionViewLayout layout)
            : base(frame, layout)
        {
        }
    }
}
