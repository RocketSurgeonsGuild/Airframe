using System;
using ReactiveUI;
using Xamarin.Forms;

namespace Rocket.Surgery.Airframe.Forms
{
    /// <summary>
    /// This is a <see cref="ListView"/> that provides observable sequences around events.
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <seealso cref="ListView" />
    /// <seealso cref="IViewFor{TViewModel}" />
    public class ReactiveListView : ListView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveListView"/> class.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="listViewCachingStrategy">The list view caching strategy.</param>
        public ReactiveListView(Type cellType, ListViewCachingStrategy listViewCachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(listViewCachingStrategy) => ItemTemplate = new DataTemplate(cellType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveListView"/> class.
        /// </summary>
        /// <param name="loadTemplate">The load template.</param>
        /// <param name="listViewCachingStrategy">The list view caching strategy.</param>
        public ReactiveListView(Func<object> loadTemplate, ListViewCachingStrategy listViewCachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(listViewCachingStrategy) => ItemTemplate = new DataTemplate(loadTemplate);

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveListView"/> class.
        /// </summary>
        /// <param name="listViewCachingStrategy">The list view caching strategy.</param>
        public ReactiveListView(ListViewCachingStrategy listViewCachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(listViewCachingStrategy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveListView"/> class.
        /// </summary>
        public ReactiveListView()
            : this(ListViewCachingStrategy.RecycleElement)
        {
        }
    }
}
