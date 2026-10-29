using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using DynamicData;

namespace Rocket.Surgery.Airframe.Data.DuckDuckGo;

/// <summary>
/// Represents a service that can query the duck duck go api.
/// </summary>
[SuppressMessage("Roslynator", "RCS1243:Duplicate word in a comment.", Justification = "Duck Duck Go")]
public class DuckDuckGoService : IDuckDuckGoService, IDisposable
{
    private readonly IDuckDuckGoApiClient _duckDuckGoApiClient;

    private readonly SourceCache<RelatedTopic, string> _queryResults =
        new SourceCache<RelatedTopic, string>(x => x.FirstUrl);

    private readonly Func<Exception, IObservable<SearchResult>> _queryException =
        _ => Observable.Empty<SearchResult>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDuckGoService"/> class.
    /// </summary>
    /// <param name="duckDuckGoApiClient">The duck duck go api.</param>
    public DuckDuckGoService(IDuckDuckGoApiClient duckDuckGoApiClient) =>
        _duckDuckGoApiClient = duckDuckGoApiClient;

    /// <inheritdoc/>
    public IObservable<IChangeSet<RelatedTopic, string>> Query(string query) => Observable
       .Create<IChangeSet<RelatedTopic, string>>(observer =>
                _duckDuckGoApiClient
                   .Search(query)
                   .Catch(_queryException)
                   .Select(x => x.AsResult())
                   .Cache(_queryResults, false)
                   .Subscribe(observer));

    /// <inheritdoc/>
    public IObservable<IChangeSet<RelatedTopic, string>> Query(string query, bool clearCache) => Observable
       .Create<IChangeSet<RelatedTopic, string>>(observer =>
                _duckDuckGoApiClient
                   .Search(query)
                   .Catch(_queryException)
                   .Select(x => x.AsResult())
                   .Cache(_queryResults, clearCache)
                   .Subscribe(observer));

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose of internal resources.
    /// </summary>
    /// <param name="disposing">A value indicating whether this instance is being disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _queryResults.Dispose();
        }
    }
}