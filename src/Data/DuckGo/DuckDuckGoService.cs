using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using LanguageExt;
using Refit;

namespace Data
{
    /// <summary>
    /// Represents a service that can query the duck duck go api.
    /// </summary>
    public class DuckDuckGoService : IDuckDuckGoService
    {
        private readonly IDuckDuckGoApi _duckDuckGoApi;
        private readonly SourceCache<DuckDuckGoQueryResult, string> _queryResults =
            new SourceCache<DuckDuckGoQueryResult, string>(x => x.FirstUrl);

        /// <summary>
        /// Initializes a new instance of the <see cref="DuckDuckGoService"/> class.
        /// </summary>
        /// <param name="duckDuckGoApi">The duck duck go api.</param>
        public DuckDuckGoService(IDuckDuckGoApi duckDuckGoApi) => _duckDuckGoApi = duckDuckGoApi;

        /// <inheritdoc/>
        public IObservable<IChangeSet<DuckDuckGoQueryResult, string>> QueryResults =>
            _queryResults.Connect();

        /// <inheritdoc/>
        public async Task Query(string query)
        {
            var results = await _duckDuckGoApi.Search(query).ConfigureAwait(false);
            foreach (var relatedTopic in results.RelatedTopics.Where(x => !string.IsNullOrEmpty(x.FirstUrl)))
            {
                _queryResults.AddOrUpdate(relatedTopic);
            }
        }

        public async Task Query() =>
            await _duckDuckGoApi.Search("")
                                .Map(x => x.AsResult())
                                .Cache(_queryResults)
                                .ConfigureAwait(true);
    }

    public static class DuckDuckGoFunctions
    {
        public static IDuckDuckGoApi DuckDuckGoApi() => RestService.For<IDuckDuckGoApi>("https://");

        public static IEnumerable<DuckDuckGoQueryResult> AsResult(this DuckDuckGoSearchResult nuget) => nuget.RelatedTopics.Where(x => !string.IsNullOrEmpty(x.FirstUrl));

        public static async Task Cache(this Task<IEnumerable<DuckDuckGoQueryResult>> result, SourceCache<DuckDuckGoQueryResult, string> cache) => cache.AddOrUpdate(await result.ConfigureAwait(false));
    }
}