using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData;

namespace Rocket.Surgery.Airframe.Data
{
    internal static class CacheFunctions
    {
        /// <summary>
        /// Caches the list of <see cref="T"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="clearCache">A value determining whether to clear the cache.</param>
        /// <returns>A completion notification.</returns>
        public static IObservable<IChangeSet<T, string>> Cache<T>(this IObservable<IEnumerable<T>> result, SourceCache<T, string> cache, bool clearCache)
            where T : IHaveIdentifier<string> => result
           .Do(UpdateCache(cache, clearCache))
           .Select(_ => cache.Connect().RefCount())
           .Switch();

        /// <summary>
        /// Caches the list of <see cref="T"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="cache">The cache.</param>
        /// <returns>A completion notification.</returns>
        public static IObservable<T> Cache<T>(this IObservable<T> result, SourceCache<T, string> cache)
            where T : IHaveIdentifier<string> => result
           .Do(x => Update(x, cache));

        private static void Update<T>(T item, ISourceCache<T, string> cache)
            where T : IHaveIdentifier<string> => cache.AddOrUpdate(item);

        private static Action<IEnumerable<T>> UpdateCache<T>(ISourceCache<T, string> cache, bool clearCache)
            where T : IHaveIdentifier<string> => results =>
        {
            if (clearCache)
            {
                cache
                   .Edit(updater =>
                    {
                        updater.Clear();
                        updater.AddOrUpdate(results);
                    });
            }
            else
            {
                cache.EditDiff(results, (first, second) => first.Id == second.Id);
            }
        };
    }
}