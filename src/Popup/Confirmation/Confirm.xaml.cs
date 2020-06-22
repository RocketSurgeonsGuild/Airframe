using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rocket.Surgery.Airframe.Popup.Confirmation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmPopup : PopupPageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmPopup"/> class.
        /// </summary>
        /// <param name="confirmDetail">The confirmation detail.</param>
        public ConfirmPopup(ConfirmDetailModel confirmDetail)
        {
            InitializeComponent();
            this.OneWayBind((ConfirmDetailModel) ViewModel, x => x.Message, x => x.Message.Text)
                .DisposeWith(ControlBindings);

            this.OneWayBind((ConfirmDetailModel) ViewModel, x => x.ConfirmMessage, x => x.Confirm.Text)
                .DisposeWith(ControlBindings);

            this.OneWayBind((ConfirmDetailModel) ViewModel, x => x.DeclineMessage, x => x.Cancel.Text)
                .DisposeWith(ControlBindings);

            this.OneWayBind((ConfirmDetailModel) ViewModel, x => x.Title, x => x.Title.Text)
                .DisposeWith(ControlBindings);

            Confirm
                .Events()
                .Pressed
                .Select(_ => true)
                .Merge(Cancel.Events().Pressed.Select(_ => false))
                .StartWith(false)
                .DistinctUntilChanged()
                .Subscribe(result =>
                {
                    Result = result;
                    PopupNavigation.Instance.PopAsync();
                })
                .DisposeWith(ViewBindings);

            BindingContext = confirmDetail;
        }

        public bool Result { get; set; }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ControlBindings?.Dispose();
            ViewBindings?.Dispose();
        }
    }
}