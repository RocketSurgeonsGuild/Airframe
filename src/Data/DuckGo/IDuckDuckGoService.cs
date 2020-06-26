using System;
using System.Threading.Tasks;
using DynamicData;

namespace Data
{
    /// <summary>
    /// Interface representing a service that queries the <see cref="IDuckDuckGoApi"/>.
    /// </summary>
    public interface IDuckDuckGoService
    {
        /// <summary>
        /// Gets the results of a query.
        /// </summary>
        IObservable<IChangeSet<DuckDuckGoQueryResult, string>> QueryResults { get; }

        /// <summary>
        /// Queries duck duck go.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A completion notification.</returns>
        Task Query(string query);
    }
}