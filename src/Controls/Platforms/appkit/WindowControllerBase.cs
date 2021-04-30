using System;
using System.Collections.Generic;
using System.Text;
using AppKit;
using ReactiveUI;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Base <see cref="NSWindowController"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    public abstract class WindowControllerBase<TViewModel> : ReactiveWindowController, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        private TViewModel _viewModel;

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }
    }
}
