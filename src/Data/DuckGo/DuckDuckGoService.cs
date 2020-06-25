using System;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;

namespace Data
{
    public class DuckDuckGoService : IDuckDuckGoService
    {
        private readonly IDuckDuckGoApi _duckDuckGoApi;
        private readonly SourceCache<DuckDuckGoQueryResult, string> _queryResults =
            new SourceCache<DuckDuckGoQueryResult, string>(x => x.FirstUrl);

        /// <summary>
        /// Initializes a new instance of the <see cref="DuckDuckGoService"/> class.
        /// </summary>
        /// <param name="duckDuckGoApi"></param>
        public DuckDuckGoService(IDuckDuckGoApi duckDuckGoApi)
        {
            _duckDuckGoApi = duckDuckGoApi;
        }

        public async Task Query(string query)
        {
            var results = await _duckDuckGoApi.Search(query).ConfigureAwait(false);
            foreach (var relatedTopic in results.RelatedTopics.Where(x => !string.IsNullOrEmpty(x.FirstUrl)))
            {
                _queryResults.AddOrUpdate(relatedTopic);
            }
        }

        public IObservable<IChangeSet<DuckDuckGoQueryResult, string>> QueryResults =>
            _queryResults.Connect();
    }
}