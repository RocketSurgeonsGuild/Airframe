using System;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;

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
    }
}