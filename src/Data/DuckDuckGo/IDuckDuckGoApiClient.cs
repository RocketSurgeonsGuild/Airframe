using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace Rocket.Surgery.Airframe.Data.DuckDuckGo
{
    /// <summary>
    /// Interface that defines a duck duck go api.
    /// </summary>
    public interface IDuckDuckGoApiClient
    {
        /// <summary>
        /// Search the duck duck go api with the provided query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Get("/?q={query}&format=json")]
        IObservable<SearchResult> Search(string query);

        /// <summary>
        /// Search the duck duck go api with the provided query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Get("/?q={query}&format=json")]
        IObservable<SearchResult> Search(string query, CancellationToken cancellationToken);
    }
}