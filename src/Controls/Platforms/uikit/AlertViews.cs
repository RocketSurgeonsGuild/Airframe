using System;
using GlobalToast;
using UIKit;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Alerts.
    /// </summary>
    public static class AlertViews
    {
        /// <summary>
        /// Displays a toast message.
        /// </summary>
        /// <param name="title">The ttitle for the toast message.</param>
        /// <param name="message">The message to display in the toast.</param>
        /// <param name="backgroundColor">The background color of the toast.</param>
        /// <param name="duration">The duration to display the toast.</param>
        /// <param name="position">The position for the toast to display.</param>
        public static void ShowToast(string title, string message, UIColor backgroundColor, double duration = 2.0, ToastPosition position = ToastPosition.Center) => Toast
           .MakeToast(message)
           .SetTitle(title)
           .SetPosition(position)
           .SetDuration(duration)
           .SetAutoDismiss(true)
           .Show();

        /// <summary>
        /// Displays a toast message.
        /// </summary>
        /// <param name="title">The title for the toast message.</param>
        /// <param name="message">The message to display in the toast.</param>
        /// <param name="appearance">The appearance.</param>
        /// <param name="duration">The duration to display the toast.</param>
        /// <param name="position">The position for the toast to display.</param>
        public static void ShowToast(string title, string message, ToastAppearance appearance, double duration = 2.0, ToastPosition position = ToastPosition.Center) => Toast
           .MakeToast(message)
           .SetTitle(title)
           .SetAppearance(appearance)
           .SetPosition(position)
           .SetDuration(duration)
           .SetAutoDismiss(true)
           .Show();

        /// <summary>
        /// Displays a toast message.
        /// </summary>
        /// <param name="title">The title for the toast message.</param>
        /// <param name="message">The message to display in the toast.</param>
        /// <param name="appearance">The appearance.</param>
        /// <param name="duration">The duration to display the toast.</param>
        /// <param name="position">The position for the toast to display.</param>
        public static void ShowToast(string title, string message, Func<ToastAppearance> appearance, double duration = 2.0, ToastPosition position = ToastPosition.Center) => Toast
           .MakeToast(message)
           .SetTitle(title)
           .SetAppearance(appearance())
           .SetPosition(position)
           .SetDuration(duration)
           .SetAutoDismiss(true)
           .Show();
    }
}