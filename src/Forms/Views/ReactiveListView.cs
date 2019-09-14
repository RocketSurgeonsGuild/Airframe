using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using Xamarin.Forms;

namespace Rocket.Surgery.ReactiveUI.Forms
{
    /// <summary>
    /// This is a <see cref="ListView"/> that has extends <see cref="IViewFor"/>.
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ListView" />
    /// <seealso cref="IViewFor{TViewModel}" />
    public class ReactiveListView<TViewModel> : ListView, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// The view model bindable property.
        /// </summary>
        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ListViewBase<TViewModel>),
            default(TViewModel),
            BindingMode.OneWay,
            propertyChanged: OnViewModelChanged);

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveListView{TViewModel}"/> class.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="listViewCachingStrategy">The list view caching strategy.</param>
        public ReactiveListView(Type cellType, ListViewCachingStrategy listViewCachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(listViewCachingStrategy)
        {
            ItemTemplate = new DataTemplate(cellType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveListView{TViewModel}"/> class.
        /// </summary>
        /// <param name="loadTemplate">The load template.</param>
        /// <param name="listViewCachingStrategy">The list view caching strategy.</param>
        public ReactiveListView(Func<object> loadTemplate, ListViewCachingStrategy listViewCachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(listViewCachingStrategy)
        {
            ItemTemplate = new DataTemplate(loadTemplate);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveListView{TViewModel}"/> class.
        /// </summary>
        /// <param name="listViewCachingStrategy">The list view caching strategy.</param>
        public ReactiveListView(ListViewCachingStrategy listViewCachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(listViewCachingStrategy)
        {
        }

        /// <summary>
        /// Gets or sets the ViewModel corresponding to this specific View. This should be
        /// a DependencyProperty if you're using XAML.
        /// </summary>
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        /// <summary>
        /// Gets or sets the ViewModel corresponding to this specific View. This should be
        /// a DependencyProperty if you're using XAML.
        /// </summary>
        public TViewModel ViewModel { get; set; }

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

        /// <inheritdoc/>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            ViewModel = BindingContext as TViewModel;
        }

        private static void OnViewModelChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            bindableObject.BindingContext = newValue;
        }
    }
}
