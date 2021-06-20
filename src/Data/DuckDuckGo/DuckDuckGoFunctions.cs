using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;

namespace Rocket.Surgery.Airframe.Data.DuckDuckGo
{
    /// <summary>
    /// <see cref="IDuckDuckGoApiClient"/> functions.
    /// </summary>
    public static class DuckDuckGoFunctions
    {
        /// <summary>
        /// Converts a <see cref="SearchResult"/> to an enumerable of <see cref="RelatedTopic"/>.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        /// <returns>The enumeration of quest results.</returns>
        public static IEnumerable<RelatedTopic> AsResult(this SearchResult searchResult)
            => searchResult.RelatedTopics.Where(x => !string.IsNullOrEmpty(x.FirstUrl));

        /// <summary>
        /// Caches the list of <see cref="RelatedTopic"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="cache">The cache.</param>
        /// <returns>A completion notification.</returns>
        public static IObservable<IChangeSet<RelatedTopic, string>> Cache(
            this IObservable<IEnumerable<RelatedTopic>> result,
            SourceCache<RelatedTopic, string> cache) => result
           .Do(duckDuckGoQueryResults => cache.EditDiff(duckDuckGoQueryResults, (first, second) => first.FirstUrl == second.FirstUrl))
           .SelectMany(_ => cache.Connect().RefCount());
    }
}