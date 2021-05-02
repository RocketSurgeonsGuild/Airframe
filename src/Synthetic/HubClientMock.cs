using System.Threading.Tasks;
using Rocket.Surgery.Airframe.Data;

namespace Rocket.Surgery.Airframe.Synthetic
{
    /// <summary>
    /// Represents a SignalR a hub client.
    /// </summary>
    public class HubClientMock : HubClientBase
    {
        /// <inheritdoc/>
        public override Task Connect() => Task.CompletedTask;

        /// <inheritdoc/>
        public override Task<T> InvokeAsync<T>(string methodName) => Task.FromResult(default(T));
    }
}