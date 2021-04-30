using System;
using System.Reactive.Linq;
using DynamicData;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Represents a service that can query the duck duck go api.
    /// </summary>
    public class DuckDuckGoService : IDuckDuckGoService
    {
        private readonly IDuckDuckGoApiClient _duckDuckGoApiClient;

        private readonly SourceCache<DuckDuckGoQueryResult, string> _queryResults =
            new SourceCache<DuckDuckGoQueryResult, string>(x => x.FirstUrl);

        /// <summary>
        /// Initializes a new instance of the <see cref="DuckDuckGoService"/> class.
        /// </summary>
        /// <param name="duckDuckGoApiClient">The duck duck go api.</param>
        public DuckDuckGoService(IDuckDuckGoApiClient duckDuckGoApiClient) => _duckDuckGoApiClient = duckDuckGoApiClient;

        /// <inheritdoc/>
        public IObservable<DuckDuckGoQueryResult> Query(string query) =>
            Observable
               .Create<DuckDuckGoQueryResult>(observer =>
                    _duckDuckGoApiClient
                       .Search(query)
                       .Select(x => x.AsResult())
                       .Cache(_queryResults)
                       .SelectMany(x => x)
                       .Subscribe(observer));

        public IObservable<DuckDuckGoQueryResult> Query(string query, bool clearCache) => 
            Observable
               .Create<DuckDuckGoQueryResult>(observer =>
                    _duckDuckGoApiClient
                       .Search(query)
                       .Select(x => x.AsResult())
                       .Cache(_queryResults)
                       .SelectMany(x => x)
                       .Subscribe(observer));;
    }
}