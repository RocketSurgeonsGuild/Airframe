using System;
using System.Threading.Tasks;
using UIKit;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Background task for iOS.
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/en-us/xamarin/ios/app-fundamentals/backgrounding/ios-backgrounding-techniques/ios-backgrounding-with-tasks.
    /// </remarks>
    public class IosBackgroundOperation : IBackgroundOperation
    {
        /// <inheritdoc />
        public async Task ExecuteInBackground(Func<Task> backgroundTask)
        {
            nint taskID = UIApplication.SharedApplication.BeginBackgroundTask(() => { });

            try
            {
                await backgroundTask.Invoke().ConfigureAwait(false);
            }
            finally
            {
                UIApplication.SharedApplication.EndBackgroundTask(taskID);
            }
        }

        /// <inheritdoc cref="IBackgroundOperation" />
        public async Task<T> ExecuteInBackground<T>(Func<Task<T>> backgroundTask)
        {
            nint taskID = UIApplication.SharedApplication.BeginBackgroundTask(() => { });

            T result = default(T);

            try
            {
                result = await backgroundTask.Invoke().ConfigureAwait(false);
            }
            finally
            {
                UIApplication.SharedApplication.EndBackgroundTask(taskID);
            }

            return result;
        }
    }
}