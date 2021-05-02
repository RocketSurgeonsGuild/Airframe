using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// <see cref="IDuckDuckGoApiClient"/> functions.
    /// </summary>
    public static class DuckDuckGoFunctions
    {
        /// <summary>
        /// Converts a <see cref="DuckDuckGoSearchResult"/> to an enumerable of <see cref="DuckDuckGoQueryResult"/>.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        /// <returns>The enumeration of quest results.</returns>
        public static IEnumerable<DuckDuckGoQueryResult> AsResult(this IEnumerable<DuckDuckGoSearchResult> searchResult)
            => searchResult.SelectMany(result => result.RelatedTopics.Where(x => !string.IsNullOrEmpty(x.FirstUrl)));

        /// <summary>
        /// Caches the list of <see cref="DuckDuckGoQueryResult"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="clearCache">A value indicating whether to clear the cache.</param>
        /// <returns>A completion notification.</returns>
        public static IObservable<IChangeSet<DuckDuckGoQueryResult, string>> Cache(
            this IObservable<IEnumerable<DuckDuckGoQueryResult>> result,
            SourceCache<DuckDuckGoQueryResult, string> cache,
            bool clearCache = false) => result
           .Do(duckDuckGoQueryResults =>
                {
                    if (clearCache)
                    {
                        cache.Clear();
                    }

                    cache.AddOrUpdate(duckDuckGoQueryResults);
                })
           .SelectMany(_ => cache.Connect().RefCount());
    }
}