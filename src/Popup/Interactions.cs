using System.Reactive;
using ReactiveUI;
using Rocket.Surgery.Airframe.Popup.Actions;
using Rocket.Surgery.Airframe.Popup.Alerts;
using Rocket.Surgery.Airframe.Popup.Confirmation;

namespace Rocket.Surgery.Airframe.Popup
{
    public static class Interactions
    {
        /// <summary>
        /// Gets an <see cref="Interaction{TInput,TOutput}"/> that displays an alert.
        /// </summary>
        public static readonly Interaction<AlertDetailModel, Unit> ShowAlert = new Interaction<AlertDetailModel, Unit>();

        /// <summary>
        /// Gets an <see cref="Interaction{TInput,TOutput}"/> that displays an action sheet.
        /// </summary>
        public static readonly Interaction<ActionSheetModel, string> ShowActionSheet = new Interaction<ActionSheetModel, string>(RxApp.MainThreadScheduler);

        /// <summary>
        /// Gets an <see cref="Interaction{TInput,TOutput}"/> that displays a confirmation.
        /// </summary>
        public static readonly Interaction<ConfirmDetailModel, bool> ShowConfirmation = new Interaction<ConfirmDetailModel, bool>();
    }
}