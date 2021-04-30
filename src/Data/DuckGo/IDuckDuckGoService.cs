using System;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Interface representing a service that queries the <see cref="IDuckDuckGoApiClient"/>.
    /// </summary>
    public interface IDuckDuckGoService
    {
        /// <summary>
        /// Queries duck duck go.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>An observable of chang sets.</returns>
        IObservable<DuckDuckGoQueryResult> Query(string query);

        /// <summary>
        /// Queries duck duck go.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="clearCache">A value indicating whether to clear the cache.</param>
        /// <returns>An observable of chang sets.</returns>
        IObservable<DuckDuckGoQueryResult> Query(string query, bool clearCache);
    }
}