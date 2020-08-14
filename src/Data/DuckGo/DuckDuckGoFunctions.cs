using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using Refit;

namespace Data
{
    /// <summary>
    /// <see cref="DuckDuckGoApi"/> functions.
    /// </summary>
    public static class DuckDuckGoFunctions
    {
        /// <summary>
        /// Converts a <see cref="DuckDuckGoSearchResult"/> to an enumerable of <see cref="DuckDuckGoQueryResult"/>.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        /// <returns>The enumeration of quest results.</returns>
        public static IEnumerable<DuckDuckGoQueryResult> AsResult(this DuckDuckGoSearchResult searchResult) =>
            searchResult.RelatedTopics.Where(x => !string.IsNullOrEmpty(x.FirstUrl));

        /// <summary>
        /// Caches the list of <see cref="DuckDuckGoQueryResult"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="cache">The cache.</param>
        /// <returns>A completion notification.</returns>
        public static async Task Cache(this Task<IEnumerable<DuckDuckGoQueryResult>> result, SourceCache<DuckDuckGoQueryResult, string> cache) =>
            cache.AddOrUpdate(await result.ConfigureAwait(false));
    }
}