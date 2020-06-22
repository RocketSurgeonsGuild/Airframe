using System;
using System.Reactive.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rocket.Surgery.Airframe.Popup.Actions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionSheetDetail : ContentView
    {
        public ActionSheetDetail()
        {
            InitializeComponent();

            Tapped = TapGesture.Events().Tapped.Select(_ => new TappedEventArgs(this));
        }

        /// <summary>
        /// 
        /// </summary>
        public TapGestureRecognizer TapGesture { get; } = new TapGestureRecognizer();

        public IObservable<TappedEventArgs> Tapped { get; }
    }
}