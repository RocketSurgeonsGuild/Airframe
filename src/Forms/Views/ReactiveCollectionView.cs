using System;
using Xamarin.Forms;

namespace Rocket.Surgery.Airframe.Forms
{
    /// <summary>
    /// This is a <see cref="CollectionView"/> that provides observable sequences around events.
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <seealso cref="CollectionView" />
    public class ReactiveCollectionView : CollectionView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionView"/> class.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        public ReactiveCollectionView(Type cellType) => ItemTemplate = new DataTemplate(cellType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionView"/> class.
        /// </summary>
        /// <param name="loadTemplate">The load template.</param>
        public ReactiveCollectionView(Func<object> loadTemplate) => ItemTemplate = new DataTemplate(loadTemplate);
    }
}