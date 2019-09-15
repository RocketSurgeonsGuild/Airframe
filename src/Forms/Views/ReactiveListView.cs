using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using Xamarin.Forms;

namespace Rocket.Surgery.ReactiveUI.Forms
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
            : base(listViewCachingStrategy)
        {
            ItemTemplate = new DataTemplate(cellType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveListView"/> class.
        /// </summary>
        /// <param name="loadTemplate">The load template.</param>
        /// <param name="listViewCachingStrategy">The list view caching strategy.</param>
        public ReactiveListView(Func<object> loadTemplate, ListViewCachingStrategy listViewCachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(listViewCachingStrategy)
        {
            ItemTemplate = new DataTemplate(loadTemplate);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveListView"/> class.
        /// </summary>
        /// <param name="listViewCachingStrategy">The list view caching strategy.</param>
        public ReactiveListView(ListViewCachingStrategy listViewCachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(listViewCachingStrategy)
        {
        }

        /// <summary>
        /// Gets the an observable sequence of item appearing events.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>An observable sequence of items appearing.</returns>
        /// <value>The ListView item appearing.</value>
        public IObservable<TViewModel> ListViewItemAppearing<TViewModel>()
            where TViewModel : class =>
                Observable
                    .FromEvent<EventHandler<ItemVisibilityEventArgs>, ItemVisibilityEventArgs>(x => ItemDisappearing += x, x => ItemDisappearing -= x)
                    .Select(x => x.Item as TViewModel);

        /// <summary>
        /// Gets the an observable sequence of item disappearing events.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>An observable sequence of items disappearing.</returns>
        /// <value>The ListView item appearing.</value>
        public IObservable<TViewModel> ListViewItemDisappearing<TViewModel>()
            where TViewModel : class =>
                Observable
                    .FromEvent<EventHandler<ItemVisibilityEventArgs>, ItemVisibilityEventArgs>(x => ItemAppearing += x, x => ItemAppearing -= x)
                    .Select(x => x.Item as TViewModel);

        /// <summary>
        /// Gets an observable sequence of the item tapped from the <see cref="ListView"/>.
        /// </summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <returns>An observable sequence of items selected.</returns>
        public IObservable<T> ListViewItemTapped<T>()
            where T : class =>
                Observable
                    .FromEvent<EventHandler<ItemTappedEventArgs>, ItemTappedEventArgs>(x => ItemTapped += x, x => ItemTapped -= x)
                    .Select(args => args.Item as T);

        /// <summary>
        /// Gets an observable sequence of the item tapped from the <see cref="ListView"/>.
        /// </summary>
        /// <returns>An observable sequence of items selected.</returns>
        public IObservable<object> ListViewItemTapped() =>
            Observable
                .FromEvent<EventHandler<ItemTappedEventArgs>, ItemTappedEventArgs>(x => ItemTapped += x, x => ItemTapped -= x)
                .Select(args => args.Item);

        /// <summary>
        /// Gets an observable sequence of the item selected from the <see cref="ListView"/>.
        /// </summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <returns>An observable sequence of items selected.</returns>
        public IObservable<T> ListViewItemSelected<T>()
            where T : class =>
                Observable
                    .FromEvent<EventHandler<SelectedItemChangedEventArgs>, SelectedItemChangedEventArgs>(x => ItemSelected += x, x => ItemSelected -= x)
                    .Select(args => args.SelectedItem as T);

        /// <summary>
        /// Gets an observable sequence of the item selected from the <see cref="ListView"/>.
        /// </summary>
        /// <returns>An observable sequence of items selected.</returns>
        public IObservable<object> ListViewItemSelected() =>
            Observable
                .FromEvent<EventHandler<SelectedItemChangedEventArgs>, SelectedItemChangedEventArgs>(x => ItemSelected += x, x => ItemSelected -= x)
                .Select(args => args.SelectedItem);
    }
}
