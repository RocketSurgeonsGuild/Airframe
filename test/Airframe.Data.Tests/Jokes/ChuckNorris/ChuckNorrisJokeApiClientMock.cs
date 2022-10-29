using Rocket.Surgery.Airframe.Data.ChuckNorris;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Airframe.Data.Tests.Jokes.ChuckNorris;

public class ChuckNorrisJokeApiClientMock : IChuckNorrisJokeApiClient
{
    private readonly IObservable<IEnumerable<ChuckNorrisJoke>> _jokeClient;
    private Subject<IEnumerable<ChuckNorrisJoke>> _jokes = new Subject<IEnumerable<ChuckNorrisJoke>>();

    public ChuckNorrisJokeApiClientMock() => _jokeClient = _jokes.AsObservable();

    public IObservable<ChuckNorrisJoke> Random() => Observable.Create<ChuckNorrisJoke>(observer => _jokes.SelectMany(jokes => jokes).Subscribe(observer));

    public IObservable<IEnumerable<ChuckNorrisJoke>> RandomFromCategory(string category) => Observable.Create<IEnumerable<ChuckNorrisJoke>>(observer => _jokes.Select(jokes => jokes.Where(x => x.Categories.Contains(category))).Subscribe(observer));

    public IObservable<IEnumerable<string>> Categories() => null;

    public IObservable<IEnumerable<ChuckNorrisJoke>> Search(string query)
        => Observable.Create<IEnumerable<ChuckNorrisJoke>>(observer => _jokes.Subscribe(observer));

    public void Notify(params ChuckNorrisJoke[] jokes) => _jokes.OnNext(jokes);
}