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
        /// <param name="clearCache">A value determining whether to clear the cache.</param>
        /// <returns>A completion notification.</returns>
        public static IObservable<IChangeSet<RelatedTopic, string>> Cache(
            this IObservable<IEnumerable<RelatedTopic>> result,
            SourceCache<RelatedTopic, string> cache,
            bool clearCache) => result
           .Do(UpdateCache(cache, clearCache))
           .Select(_ => cache.Connect().RefCount())
           .Switch();

        private static Action<IEnumerable<RelatedTopic>> UpdateCache(SourceCache<RelatedTopic, string> cache, bool clearCache) => duckDuckGoQueryResults =>
        {
            if (clearCache)
            {
                cache
                   .Edit(updater =>
                        {
                            updater.Clear();
                            updater.AddOrUpdate(duckDuckGoQueryResults);
                        });
            }
            else
            {
                cache.EditDiff(duckDuckGoQueryResults, (first, second) => first.FirstUrl == second.FirstUrl);
            }
        };
    }
}