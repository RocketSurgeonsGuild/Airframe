using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;

namespace Rocket.Surgery.Airframe.Data;

/// <summary>
/// Represents a service that produces <see cref="ChuckNorrisJoke"/>.
/// </summary>
public class ChuckNorrisJokeService : IChuckNorrisJokeService
{
    private readonly IChuckNorrisJokeApiClient _chuckNorrisJokeApiClient;
    private readonly SourceCache<ChuckNorrisJoke, string> _jokes = new SourceCache<ChuckNorrisJoke, string>(x => x.Id);

    /// <summary>
    /// Initializes a new instance of the <see cref="ChuckNorrisJokeService"/> class.
    /// </summary>
    /// <param name="chuckNorrisJokeApiClient">The api client.</param>
    public ChuckNorrisJokeService(IChuckNorrisJokeApiClient chuckNorrisJokeApiClient) => _chuckNorrisJokeApiClient = chuckNorrisJokeApiClient;

    /// <inheritdoc/>
    public IObservable<ChuckNorrisJoke> Random() => Observable
       .Create<ChuckNorrisJoke>(observer =>
            _chuckNorrisJokeApiClient
               .Random()
               .Cache(_jokes)
               .Subscribe(observer));

    /// <inheritdoc/>
    public IObservable<ChuckNorrisJoke> Random(params string[] categories) => Observable.Create<ChuckNorrisJoke>(
        observer =>
        {
            var disposable = new CompositeDisposable();

            foreach (var category in categories)
            {
                _chuckNorrisJokeApiClient
                   .RandomFromCategory(category)
                   .SelectMany(jokes => jokes)
                   .Cache(_jokes)
                   .Subscribe(observer)
                   .DisposeWith(disposable);
            }

            return disposable;
        });

    /// <inheritdoc/>
    public IObservable<IChangeSet<ChuckNorrisJoke, string>> Query(string query) => Query(query, true);

    /// <inheritdoc/>
    public IObservable<IChangeSet<ChuckNorrisJoke, string>> Query(string query, bool clearCache) => Observable
       .Create<IChangeSet<ChuckNorrisJoke, string>>(
            observer =>
                _chuckNorrisJokeApiClient
                   .Search(query)
                   .Cache(_jokes, clearCache)
                   .Subscribe(observer));
}