using System;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rocket.Surgery.Airframe.Popup.Alerts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Alert : PopupPageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alert"/> class.
        /// </summary>
        /// <param name="alertDetail">The alert detail.</param>
        public Alert(AlertDetailModel alertDetail)
        {
            InitializeComponent();

            BindingContext = alertDetail;

            Confirm
                .Events()
                .Pressed
                .Subscribe(_ => PopupNavigation.Instance.PopAsync());

            // Header.Source = viewModel.HeaderImage;
            // Title.Text = viewModel.Title;
            // Message.Text = viewModel.Message;
            // Completed = Observable
            //     .Timer(viewModel.Timeout, RxApp.TaskpoolScheduler)
            //     .TakeUntil(_cancelled)
            //     .ObserveOn(RxApp.MainThreadScheduler)
            //     .Select(_ => Pop())
            //     .Switch()
            //     .Publish()
            //     .RefCount();
        }
    }
}