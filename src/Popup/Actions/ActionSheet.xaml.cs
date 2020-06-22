using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rocket.Surgery.Airframe.Popup.Actions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionSheet : PopupPageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionSheet"/> class.
        /// </summary>
        /// <param name="actionSheetDetail">The action sheet detail.</param>
        public ActionSheet(ActionSheetModel actionSheetDetail)
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.BindingContext)
                .Cast<ActionSheetModel>()
                .Where(x => x.Buttons != null)
                .Select(x => x.Buttons)
                .BindTo(this, x => x.Sheet.ItemsSource)
                .DisposeWith(ControlBindings);

            BindingContext = actionSheetDetail;
        }

        public TapGestureRecognizer Tapped { get; }

        public string Result { get; set; }
    }
}