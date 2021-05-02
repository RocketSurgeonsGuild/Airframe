using System;
using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Base abstraction for hub clients.
    /// </summary>
    public abstract class HubClientBase : IHubClient
    {
        /// <inheritdoc/>
        public abstract Task Connect();

        /// <inheritdoc/>
        public abstract Task<T> InvokeAsync<T>(string methodName);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources.
        /// </summary>
        /// <param name="disposing">A value indicating disposal.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}