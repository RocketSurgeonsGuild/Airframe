using System;
using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Interface representing an operation to run in the background on a specific platform.
    /// </summary>
    public interface IBackgroundOperation
    {
        /// <summary>
        /// Processes the task on the platform specific background thread.
        /// </summary>
        /// <param name="operation">The background operation.</param>
        /// <returns>A completion notification.</returns>
        Task ExecuteInBackground(Func<Task> operation);

        /// <summary>
        /// Processes the task on the platform specific background thread.
        /// </summary>
        /// <param name="operation">The background operation.</param>
        /// <typeparam name="T">The return type.</typeparam>
        /// <returns>A typed completion notification.</returns>
        Task<T> ExecuteInBackground<T>(Func<Task<T>> operation);
    }
}