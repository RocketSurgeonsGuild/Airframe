using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using Xamarin.Forms;

namespace Rocket.Surgery.ReactiveUI.Forms
{
    /// <summary>
    /// Base ReactiveUI <see cref="ReactiveListView{TViewModel}" />.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveListView{TViewModel}" />
    /// <seealso cref="ListView" />
    public abstract class ListViewBase<TViewModel> : ReactiveListView<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="listViewCachingStrategy">The list view caching strategy.</param>
        protected ListViewBase(Type cellType, ListViewCachingStrategy listViewCachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(cellType, listViewCachingStrategy)
        {
            ListViewItemAppearing =
                Observable
                    .FromEvent<EventHandler<ItemVisibilityEventArgs>, ItemVisibilityEventArgs>(x => ItemAppearing += x, x => ItemAppearing -= x)
                    .Select(x => x.Item as TViewModel);

            ListViewItemDisappearing =
                Observable
                    .FromEvent<EventHandler<ItemVisibilityEventArgs>, ItemVisibilityEventArgs>(x => ItemDisappearing += x, x => ItemDisappearing -= x)
                    .Select(x => x.Item as TViewModel);
        }

        /// <summary>
        /// Gets the an observable sequence of item appearing events.
        /// </summary>
        /// <value>The ListView item appearing.</value>
        public IObservable<TViewModel> ListViewItemAppearing { get; }

        /// <summary>
        /// Gets the an observable sequence of item disappearing events.
        /// </summary>
        public IObservable<TViewModel> ListViewItemDisappearing { get; }

        /// <summary>
        /// Gets the view bindings disposable.
        /// </summary>
        protected CompositeDisposable ViewBindings { get; } = new CompositeDisposable();
    }
}
