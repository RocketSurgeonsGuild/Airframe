using System;
using System.Reactive.Linq;
using Foundation;
using static AppKit.NSApplication;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Observable extensions for user notification.
    /// </summary>
    public static class UserNotificationExtensions
    {
        private static readonly NSUserNotificationCenter UserNotificationCenter;

        static UserNotificationExtensions()
        {
            UserNotificationCenter = NSUserNotificationCenter.DefaultUserNotificationCenter;

            // Make sure the notification fires even if the app is TopMost
            NSUserNotificationCenter.DefaultUserNotificationCenter.ShouldPresentNotification = (c, n) => true;
        }

        /// <summary>
        /// Notifies the user.
        /// </summary>
        /// <typeparam name="T">The source type.</typeparam>
        /// <param name="sender">The sender.</param>
        /// <param name="title">The title.</param>
        /// <param name="informativeText">The informative text.</param>
        /// <returns>The source observable.</returns>
        public static IObservable<T> NotifyUser<T>(this IObservable<T> sender, string title, string informativeText)
        {
            var notification = new NSUserNotification
            {
                Title = title,
                InformativeText = informativeText,
                DeliveryDate = (NSDate)DateTime.Now,
                SoundName = NSUserNotification.NSUserNotificationDefaultSoundName,
                HasActionButton = true
            };

            UserNotificationCenter.ScheduleNotification(notification);
            return sender;
        }

        /// <summary>
        /// Notifies the user.
        /// </summary>
        /// <typeparam name="T">The source type.</typeparam>
        /// <param name="sender">The sender.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>The source observable.</returns>
        public static IObservable<T> NotifyUser<T>(this IObservable<T> sender, Func<NSUserNotification> factory)
        {
            UserNotificationCenter.ScheduleNotification(factory());
            return sender;
        }
    }
}